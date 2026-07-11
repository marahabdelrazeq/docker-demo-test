using CommonLibrary.RequestInformation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CommonRepo.Domain.Interfaces;
using CommonRepo.Infrastructure.Caching.Models;
using CommonRepo.Infrastructure.Configurations;
using CommonRepo.Infrastructure.Repositories;
using Redis.OM;
using Redis.OM.Modeling;
using Redis.OM.Searching;
using System.Reflection;
using CommonRepo.Persistence.Context;

namespace CommonRepo.Infrastructure.Caching
{
    public class IndexCreationService : IHostedService
    {
        private readonly RedisConnectionProvider _provider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRedisCollection<TablesWithViews> _redisCollection;
        private readonly ApplicationDbContext _db;
        private readonly RedisConfiguration _redisConfiguration;
        private readonly KafkaSyncProducerConfiguration _kafkaSyncProducerConfiguration;
        private readonly IRequestInfoService _requestInfoService;

        public IndexCreationService(IServiceProvider serviceProvider, ApplicationDbContext db, RedisConfiguration redisConfiguration, KafkaSyncProducerConfiguration kafkaSyncProducerConfiguration, IRequestInfoService requestInfoService)
        {
            _db = db;
            _redisConfiguration = redisConfiguration;
            _kafkaSyncProducerConfiguration = kafkaSyncProducerConfiguration;
            _requestInfoService = requestInfoService;
            _serviceProvider = serviceProvider;
            _provider = new RedisConnectionProvider(_redisConfiguration.Config);
            _redisCollection = _provider.RedisCollection<TablesWithViews>();

        }


        /// <summary>
        /// Checks redis to see if the index already exists, if it doesn't create a new index
        /// </summary>
        /// <param name="cancellationToken"></param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var entities = typeof(ApplicationDbContext).Assembly.GetTypes()
  .Where(x => x.IsClass && !x.IsAbstract && x.GetCustomAttribute<DocumentAttribute>() != null)
  .Select(x => new { name = x.GetCustomAttribute<DocumentAttribute>().IndexName, type = x }).ToList();

            // check if the index already exists or not create it
            foreach (var entity in entities)
            {
                var indexExistds = (await _provider.Connection.ExecuteAsync("FT._LIST")).ToArray().Select(x => x.ToString()).Any(x => x == entity.name);
                if (!indexExistds)
                {
                    // check if properties of entity has any index attribute and RedisIdField attribute or not
                    var properties = entity.type.GetProperties().Where(x => x.GetCustomAttribute<IndexedAttribute>() != null || x.GetCustomAttribute<RedisIdFieldAttribute>() != null).ToList();

                    // if properties count is 0  i will insert log to tell me that this entity has no index attribute or RedisIdField attribute
                    if (properties.Count == 0)
                    {
                        // check if IndexProblem exists in redis or not
                        var indexProblemExists = (await _provider.Connection.ExecuteAsync("FT._LIST")).ToArray().Select(x => x.ToString()).Any(x => x == "IndexProblem");
                        // if not exists i will create it
                        if (!indexProblemExists)
                            // create the index for IndexProblem and add prefix to the index by execute the command                            
                            await _provider.Connection.ExecuteAsync("FT.CREATE", "IndexProblem", "ON", "HASH", "PREFIX", "1", "Problem:", "SCHEMA", "Id", "NUMERIC", "SORTABLE", "IndexName", "TEXT", "SORTABLE", "Problem", "TEXT", "SORTABLE");

                        // add prefix insert the log to redis
                        await _provider.Connection.ExecuteAsync("FT.ADD", "IndexProblem", $"Problem:{Guid.NewGuid().ToString()}", "1.0", "FIELDS", "IndexName", entity.name, "Problem", "This entity has no index attribute or RedisIdField attribute");

                    }
                    // else i will create the index
                    else
                    {
                        // create the index
                        await _provider.Connection.CreateIndexAsync(entity.type);
                    }

                }
            }



            // For Test   var InfoAllIndesx = (await _provider.Connection.ExecuteAsync("FT._LIST")).ToArray().Select(x => x.ToString());

            // check if the index already exists
            var indexExists = (await _provider.Connection.ExecuteAsync("FT._LIST")).ToArray().Select(x => x.ToString()).Any(x => x == "TablesWithViews");
            if (!indexExists)
                // Create the index
                await _provider.Connection.CreateIndexAsync(typeof(TablesWithViews));

            // create scope to get the database 
            using (var scope = _serviceProvider.CreateScope())
            {
                // service provider to get the database
                //var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var context = _db;
                // Get the database name
                string databaseName = context.Database.GetDbConnection().Database;
                // get the old index
                var old = await _redisCollection.Where(x => x.DB_NAME == databaseName).ToListAsync();
                // delete the old index
                await _redisCollection.DeleteAsync(old);
                var tablesWithViews = new List<TablesWithViews>();
                // Fetch all tables
                var tableNames = context.Model.GetEntityTypes().Select(t => t.GetTableName()).ToList();
                var tabledNames = context.Model.GetEntityTypes().ToList();
                //stored every table in database i will insert it to in redis cache


                foreach (var item in entities)
                {

                    Type entityType = FindTypeInAssemblies(item.type.Name);

                    Type genericType = typeof(CacheRepository<>).MakeGenericType(entityType);
                    var instance = Activator.CreateInstance(genericType, new object[] { context, _redisConfiguration, _kafkaSyncProducerConfiguration, _requestInfoService });

                    // get method GetAllAsync from BaseCacheRepository                   

                    // get method CreationData from BaseCacheRepository
                    MethodInfo method = genericType.GetMethod("CreationData");
                    // invoke method GetAllAsync
                    var resultTask = (Task)method.Invoke(instance, new object[] { false });
                    await resultTask.ConfigureAwait(true);

                    //await (Task)method.Invoke(instance, null);



                }


                // Fetch related views for each table
                foreach (var entity in entities)
                {
                    var table = entity.name;
                    // Fetch views related to the current table
                    tablesWithViews = context.TablesWiths
                                       .FromSqlRaw($"SELECT *,Null as Type_NAME FROM INFORMATION_SCHEMA.VIEW_TABLE_USAGE WHERE TABLE_NAME = '{table}'").ToList();
                    // Fetch the database name related to the current table
                    foreach (var View in tablesWithViews)
                    {
                        View.Type_NAME = entity.type.Name;
                        // Fetch foreign keys related to the current table
                        var foreignKeys = context.KeyViews
                           .FromSqlRaw($"SELECT c1.name AS foreign_key_column FROM sys.sql_expression_dependencies AS d " +
                                       $"JOIN sys.foreign_key_columns AS fkc ON d.referenced_id = fkc.parent_object_id " +
                                       $"JOIN sys.columns AS c1 ON fkc.parent_object_id = c1.object_id AND fkc.parent_column_id = c1.column_id " +
                                       $"WHERE OBJECT_NAME(referenced_object_id) = '{table}' AND OBJECT_NAME(d.referencing_id) = '{View.VIEW_NAME}'").FirstOrDefault();
                        // if foreign key is not null i will add it to the current view
                        if (foreignKeys != null)
                            View.KEY = foreignKeys.foreign_key_column;
                        // if table equal view i will set the key to id split word View in the last and take the first part 
                        else if (table == View.VIEW_NAME.Split("View").First())
                            View.KEY = "Id";// key is set id defult

                        // Fetch the columns related to the current VIEW
                        var columns = context.COLUMN_NAME
                          //.FromSqlRaw($"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.VIEW_COLUMN_USAGE WHERE TABLE_NAME = '{table}' and VIEW_NAME='{View.VIEW_NAME}'").ToList();
                          .FromSqlRaw($"SELECT name +':'+  source_column name FROM sys.dm_exec_describe_first_result_set  (N'SELECT * from {View.VIEW_NAME}', null, 1)  WHERE source_table = '{table}'").ToList();
                        // if columns is not null i will add it to the current view
                        if (columns != null)
                            View.COLUMNS = string.Join(",", columns.Select(x => x.ColumnName));



                        // if view.key is null i will extract Id from columns and set it to key
                        if (View.KEY == null)
                        {
                            // split the columns 
                            if (columns.Any(x => x.ColumnName.Contains(":Id")))
                                // set fist part of the split to key
                                View.KEY = columns.FirstOrDefault(x => x.ColumnName.Contains(":Id")).ColumnName.Split(":").First();
                        }


                    }


                    // Insert the current table and its views into Redis
                    await _redisCollection.InsertAsync(tablesWithViews);
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
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
