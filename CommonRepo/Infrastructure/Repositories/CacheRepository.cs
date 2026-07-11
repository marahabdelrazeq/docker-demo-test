using CommonLibrary.RequestInformation;
using CommonRepo.Domain.Interfaces;
using CommonRepo.Infrastructure.Caching.Models;
using CommonRepo.Infrastructure.Configurations;
using CommonRepo.Infrastructure.Messaging;
using CommonRepo.Infrastructure.Messaging.Models;
using CommonRepo.Persistence.Context;
using Confluent.Kafka;
using FilterBuilder;
using Microsoft.EntityFrameworkCore;
using Redis.OM;
using Redis.OM.Searching;
using StackExchange.Redis;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;

namespace CommonRepo.Infrastructure.Repositories
{
    public class CacheRepository<TEntity> : Repository<TEntity>, ICacheRepository<TEntity> where TEntity : class, new()
    {
        public KafkaSyncProducerConfiguration KafkaAsyncConfigurations { get; set; }

        public ConfigurationOptions RedisConfigurations { get; set; }

        #region Private Fields
        private bool _indexCreated = false;

        private IRedisCollection<TEntity> _provider;

        private RedisConnectionProvider _redis;

        private readonly ConfigurationOptions _redisConfig;

        private readonly KafkaSyncProducerConfiguration _kafkaConfig;

        private readonly IRequestInfoService _requestInfoService;
        #endregion

        public CacheRepository(ApplicationDbContext context, RedisConfiguration redisConfigurations = null, KafkaSyncProducerConfiguration kafkaAsyncConfigurations = null, IRequestInfoService requestInfoService = null) : base(context, requestInfoService)
        {
            _requestInfoService = requestInfoService;

            _redisConfig = RedisConfigurations ?? redisConfigurations?.Config ?? new ConfigurationOptions();

            _kafkaConfig = KafkaAsyncConfigurations ?? kafkaAsyncConfigurations ?? new KafkaSyncProducerConfiguration();

            RedisExceptionHandler(() =>
            {
                _redis = new RedisConnectionProvider(_redisConfig);

                _provider = _redis.RedisCollection<TEntity>();

                CreateIndexAsync();
            });
        }

        #region Private Indexing Functions
        private async Task<bool> CreateIndexAsync()
        {
            if (!_indexCreated)
            {
                _indexCreated = await RedisExceptionHandlerAsync(() => _redis.Connection.CreateIndexAsync(typeof(TEntity)))!;
            }

            return _indexCreated;
        }

        #endregion

        #region Private Write Functions
        // create method data get data from database and insert redis cache
        public async Task<bool> CreationData(bool IsDelete = false, Expression<Func<TEntity, bool>> filter = null)
        {
            // if IsDelete is true then delete all data from redis cache
            if (IsDelete)
            {
                _provider.Delete(_provider.ToList());
                _redis.Connection.DropIndex(typeof(TEntity));
                CreateIndexAsync();
            }
            else
            {
                // check if data exist in redis cache
                var dataExist = await _provider.AnyAsync();
                if (dataExist)
                    return false;
            }


            // skip if Tentity name TablesWithViews
            if (typeof(TEntity).Name == "TablesWithViews")
                return false;

            // get all data from database
            List<TEntity> dbData = new List<TEntity>();
            if (filter != null)
            {
                dbData = await _context.Set<TEntity>().Where(filter).ToListAsync();
                if (dbData == null || dbData.Count == 0)
                    return false;
            }
            else
            {
                dbData = await _context.Set<TEntity>().ToListAsync();
                if (dbData == null || dbData.Count == 0)
                    return false;
            }

            // check if data every property is null set default value
            foreach (var item in dbData)
            {
                foreach (var prop in item.GetType().GetProperties())
                {
                    if (prop.GetValue(item) == null)
                    {
                        if (prop.PropertyType == typeof(string))
                            prop.SetValue(item, string.Empty);

                        else if (prop.PropertyType == typeof(int) || Nullable.GetUnderlyingType(prop.PropertyType) == typeof(int))
                            prop.SetValue(item, 0);

                        else if (prop.PropertyType == typeof(DateTime))
                            prop.SetValue(item, DateTime.Now);

                        else if (prop.PropertyType == typeof(bool))
                            prop.SetValue(item, false);

                        else if (prop.PropertyType == typeof(decimal) || Nullable.GetUnderlyingType(prop.PropertyType) == typeof(decimal))
                            prop.SetValue(item, 0.0m);

                        else if (prop.PropertyType == typeof(double) || Nullable.GetUnderlyingType(prop.PropertyType) == typeof(double))
                            prop.SetValue(item, 0.0);

                        else if (prop.PropertyType == typeof(float) || Nullable.GetUnderlyingType(prop.PropertyType) == typeof(float))
                            prop.SetValue(item, 0.0f);

                        else if (prop.PropertyType == typeof(long) || Nullable.GetUnderlyingType(prop.PropertyType) == typeof(long))
                            prop.SetValue(item, 0L);

                        else if (prop.PropertyType == typeof(object))
                            prop.SetValue(item, null);

                    }
                }
            }

            // insert data to redis cache
            var tasks = new List<Task>();

            foreach (var item in dbData)
            {
                tasks.Add(_provider.InsertAsync(item));
            }

            await Task.WhenAll(tasks);

            return true;
        }

        private async void InsertCacheAsync(TEntity T)
        {
            await SyncWithKafka(_kafkaConfig.Topics.AddTopic, MessageOperation.Add, T);
        }
        //InsertCacheAsync method to insert or update cache
        public async void InsertDataCacheAsync(TEntity T)
        {
            await _provider.InsertAsync(T);
            await _provider.SaveAsync();
        }

        public async void RemoveCache(TEntity T)
        {
            await _provider.DeleteAsync(T);
        }
        public async void RemoveCacheAll()
        {
            var data = await _provider.ToListAsync();
            await _provider.DeleteAsync(data);
        }
        public async void UpdateCache(TEntity T)
        {
            await _provider.UpdateAsync(T);
        }
        #endregion

        #region Private Read Functions
        private Task<IList<TEntity>> GetAllCacheAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            var requestInfo = _requestInfoService.GetRequestInfo();

            var filter = FilterExpression.Build(predicate).ApplyOwnership(requestInfo).CleanExpression();

            return RedisExceptionHandlerAsync(() => _provider.Where(filter).ToListAsync()!);
        }
        private Task<IList<TEntity>> GetAllCacheAsync(Expression<Func<TEntity, bool>> predicate = null, bool cacheOnly = true)
        {
            var requestInfo = _requestInfoService.GetRequestInfo();

            var filter = FilterExpression.Build(predicate).ApplyOwnership(requestInfo).CleanExpression();

            return RedisExceptionHandlerAsync(() => _provider.Where(filter).ToListAsync()!);
        }

        private Task<IList<TEntity>> GetAllCacheAsync<TFilter>(TFilter filter) where TFilter : class
        {
            return GetAllCacheAsync(FilterExpression.Build<TEntity, TFilter>(filter));
        }

        private async Task<(List<TEntity>, int)> GetAllCacheAsync<TFilter>(TFilter objDTO = null, int pageSize = 0, int pageNumber = 1, string orderBy = "Id DESC") where TFilter : class
        {
            return await RedisExceptionHandlerAsync(async () =>
            {

                var requestInfo = _requestInfoService.GetRequestInfo();

                var filter = FilterExpression.Build<TEntity, TFilter>(objDTO).ApplyOwnership(requestInfo).CleanExpression();

                var _query = _provider.Where(filter).OrderBy(new ParsingConfig(), orderBy);

                var totalRecord = _query?.Count() ?? 0;

                if (pageSize > 0)
                {
                    if (pageSize > 100)
                    {
                        pageSize = 100;
                    }

                    _query?.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
                }

                var result = _query?.ToList();
                return (result, totalRecord);
            });
        }

        private async Task<TEntity> FindCacheAsync(Expression<Func<TEntity, bool>> predicate = null, string orderBy = "Id DESC")
        {
            return await RedisExceptionHandlerAsync(async () =>
            {
                var requestInfo = _requestInfoService.GetRequestInfo();

                var _predicate = FilterExpression.Build(predicate).ApplyOwnership(requestInfo).CleanExpression();

                var result = _provider.Where(_predicate).OrderBy(new ParsingConfig(), orderBy).FirstOrDefault();

                return result;
            });
        }

        private async Task<TEntity> FindCacheAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            return await RedisExceptionHandlerAsync(() =>
            {
                var requestInfo = _requestInfoService.GetRequestInfo();
                return _provider.FirstOrDefaultAsync(filter.ApplyOwnership(requestInfo).CleanExpression());
            });
        }
        #endregion

        #region Override Functions
        public async override Task<TEntity> AddAsync(TEntity entity)
        {
            var result = await base.AddAsync(entity);
            InsertCacheAsync(result);
            return result;
        }

        public override async Task<TEntity> UpdateAsync(TEntity entity)
        {
            var result = await base.UpdateAsync(entity);
            UpdateCache(entity);
            return result;
        }

        public override async Task<(List<TEntity>, int)> GetAllAsync<TF>(TF objDTO = null, string includeProperties = null,
    int pageSize = 0, int pageNumber = 1, string orderBy = "Id DESC") where TF : class
        {
            var cacheResult = await GetAllCacheAsync(objDTO, pageSize, pageNumber, orderBy);
            if (cacheResult.Item1 != null && cacheResult.Item1?.Count > 0) return cacheResult;

            var dbResult = await base.GetAllAsync(objDTO, includeProperties, pageSize, pageNumber, orderBy);

            return dbResult;
        }

        public override async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter = null)
        {


            var cacheResult = await GetAllCacheAsync(filter);
            if (cacheResult != null && cacheResult.Count > 0)
                return cacheResult;

            var dbResult = await base.GetAllAsync(filter);

            if (cacheResult == null || cacheResult.Count == 0)
            {
                if (  dbResult.Count() != 0)
                {
                    await CreationData(true, filter);
                    var reloaded = await GetAllCacheAsync(filter);
                    if (reloaded != null && reloaded.Count > 0)
                        return reloaded;

                }
            }

            return dbResult;
        }
        public override async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter = null, bool tracked = true, string includeProperties = null, string orderBy = "Id DESC")
        {
            var cacheResult = await FindCacheAsync(filter, orderBy);
            if (cacheResult != null) return cacheResult;

            var dbResult = await base.GetAsync(filter, tracked, includeProperties, orderBy);
            return dbResult;
        }

        #endregion

        #region Handle Exception Functions
        protected TF RedisExceptionHandler<TF>(Func<TF> fun, TF defultValue = default)
        {
            try
            {
                return fun();
            }
            catch (Exception ex)
            {
                LogRedisExceptions(ex);
            }

            return defultValue;
        }

        protected async Task<TF> RedisExceptionHandlerAsync<TF>(Func<Task<TF>> fun, TF defultValue = default)
        {
            try
            {
                return await fun();
            }
            catch (Exception ex)
            {
                LogRedisExceptions(ex);
            }

            return defultValue;
        }

        protected async Task RedisExceptionHandlerAsync(Task task)
        {
            await RedisExceptionHandlerAsync(Task.Run(() => task));
        }

        protected async Task RedisExceptionHandlerAsync(Action action)
        {
            await RedisExceptionHandlerAsync(action);
        }

        protected void RedisExceptionHandler(Action action)
        {
            RedisExceptionHandler(() =>
            {
                action.Invoke();

                return this;
            });
        }

        protected void LogRedisExceptions(Exception ex)
        {
            if (ex is RedisConnectionException)
            {
                Console.WriteLine("Redis connection exception: " + ex.Message);
            }

            else if (ex is RedisTimeoutException)
            {
                Console.WriteLine("Redis timeout exception: " + ex.Message);
            }

            else if (ex is IOException)
            {

                Console.WriteLine("I/O exception: " + ex.Message);
            }

            else
            {
                Console.WriteLine("General exception: " + ex.Message);
            }
        }
        #endregion

        #region Kafka
        private async Task SyncWithKafka<T>(string topic, MessageOperation operation, T entity)
        {
            if (!_kafkaConfig.Enable || entity == null) return;

            List<string> messages = new List<string>
            {
                new MessageBrokerSender(entity, operation).Serialize()
            };

            if (operation == MessageOperation.Update) goto final;

            var provider = _redis.RedisCollection<TablesWithViews>();

            var views = provider.ToList().Where(x => x.Type_NAME == typeof(TEntity).Name);
            //var views = provider.ToList().Where(x => x.Type_NAME == typeof(TEntity).Name && x.VIEW_NAME.Contains(typeof(TEntity).Name));

            foreach (var view in views)
            {
                Type entityType = FindTypeInAssemblies(view.VIEW_NAME);

                string id = string.Empty;

                var col = view.COLUMNS.Split(",");
                foreach (var item in col)
                {
                    // find id column
                    if (item.Contains(":Id"))
                    {
                        id = item.Split(":")[0];
                    }
                }

                Type genericType = typeof(Repository<>).MakeGenericType(entityType);

                Type funcType = typeof(Func<,>).MakeGenericType(entityType, typeof(bool));

                Type parameterType = typeof(Expression<>).MakeGenericType(funcType);

                MethodInfo method = genericType.GetMethod("GetAll");

                if (method != null)
                {
                    // Create a parameter for the lambda expression
                    ParameterExpression parameter = Expression.Parameter(entityType);

                    // Create a simple predicate: e => e.SomeProperty == SomeValue
                    MemberExpression propertyAccess = Expression.PropertyOrField(parameter, id);

                    ConstantExpression constantValue = Expression.Constant(GetId(entity));

                    var constantVaelue = Expression.Convert(constantValue, propertyAccess.Type);

                    BinaryExpression equality = Expression.Equal(propertyAccess, constantVaelue);

                    //  BinaryExpression equality = Expression.Equal(propertyAccess, constantValue);

                    // Create Expression<Func<dynamicType, bool>>
                    var lambda = Expression.Lambda(Expression.GetFuncType(entityType, typeof(bool)), equality, parameter);

                    // Resolve the context and repository within the scope
                    var instance = Activator.CreateInstance(genericType, _context, _requestInfoService);

                    object[] parameters = { lambda };

                    var results = (IEnumerable<dynamic>)method.Invoke(instance, parameters)!;

                    foreach (var result in results)
                    {
                        // Assuming 'messages' is a list to store the results
                        messages.Add(new MessageBrokerSender(result, operation).Serialize());
                    }
                }
            }

        final:
            await PublishMessages(messages, topic);
        }

        private async Task PublishMessages(List<string> messages, string topic)
        {
            if (messages == null || messages.Count == 0) return;

            foreach (var message in messages)
            {
                using (var producer = new ProducerBuilder<Null, string>(_kafkaConfig.Config).Build())
                {
                    await producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
                }
            }
        }

        private Type FindTypeInAssemblies(string typeName)
        {
            // Get all loaded assemblies
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Iterate through the assemblies and try to find the type
            foreach (var assembly in assemblies)
            {
                var type = assembly.GetTypes().FirstOrDefault(t => t.Name == typeName);

                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }

        private object GetId<T>(T entity)
        {
            var idProperty = typeof(TEntity).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public);

            var id = idProperty?.GetValue(entity);

            var convertedId = Convert.ChangeType(id, idProperty.PropertyType);

            return convertedId;
        }


        #endregion
    }
}
