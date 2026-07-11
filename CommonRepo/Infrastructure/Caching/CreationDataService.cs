using CommonLibrary.RequestInformation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using CommonRepo.Domain.Interfaces;
using CommonRepo.Infrastructure.Configurations;
using Redis.OM;
using Redis.OM.Searching;

namespace CommonRepo.Infrastructure.Caching
{
    public class CreationDataService<TEntity> : ICreationDataService<TEntity>, IHostedService where TEntity : class
    {
        private readonly RedisConnectionProvider _provider;
        private readonly IRedisCollection<TEntity> _redisCollection;
        private readonly DbContext _db;
        private readonly RedisConfiguration _redisConfiguration;

        public CreationDataService(IServiceProvider serviceProvider, DbContext db, RedisConfiguration redisConfiguration, KafkaSyncProducerConfiguration kafkaSyncProducerConfiguration, IRequestInfoService requestInfoService)
        {
            _db = db;
            _redisConfiguration = redisConfiguration;
            _provider = new RedisConnectionProvider(_redisConfiguration.Config);
            _redisCollection = _provider.RedisCollection<TEntity>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_provider.Connection.GetIndexInfo(typeof(TEntity))?.IndexName is not null)
                {
                    var old = await _redisCollection.ToListAsync();
                    _redisCollection.Delete(old);
                    await _provider.Connection.DropIndexAsync(typeof(TEntity));
                }
                await _provider.Connection.CreateIndexAsync(typeof(TEntity));
                var data = await _db.Set<TEntity>().ToListAsync();
                foreach (var item in data)
                {
                    foreach (var prop in item.GetType().GetProperties())
                    {
                        if (prop.GetValue(item) == null)
                        {
                            if (prop.PropertyType == typeof(string))
                                prop.SetValue(item, string.Empty);

                            else if (prop.PropertyType == typeof(int) || (Nullable.GetUnderlyingType(prop.PropertyType) == typeof(int)))
                                prop.SetValue(item, 0);

                        }
                    }

                }
                await _redisCollection.InsertAsync(data);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken) => _provider.Connection.Dispose();

    }
}
