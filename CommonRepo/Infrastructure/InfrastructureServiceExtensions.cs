using CommonLibrary.Caching.Redis;
using CommonRepo.Domain.Interfaces;
using CommonRepo.Domain.Interfaces.SubscritionsIRepos;
using CommonRepo.Infrastructure.Configurations;
using CommonRepo.Infrastructure.Repositories;
using CommonRepo.Infrastructure.Repositories.SubscriptionRepos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommonRepo.Infrastructure;
public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRepositoriesServices();
        services.AddReportsServices();
        services.AddCachingServices(configuration);
        services.AddConsumerCachingServices(configuration);

        return services;
    }

    public static IServiceCollection AddRepositoriesServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped(typeof(ICacheRepository<>), typeof(CacheRepository<>));

        services.AddScoped<ISystemConfigurationRepository, SystemConfigurationRepository>();

        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

        services.AddScoped<IEWaybillsViewRepository, EWaybillsViewRepository>();




        // Register your repositories here



        return services;
    }

    public static IServiceCollection AddReportsServices(this IServiceCollection services)
    {
      
        return services;
    }

    public static IServiceCollection AddCachingServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new RedisCacheEngine(configuration?.GetValue<string>("RedisConnection")));

        var KafkaSyncProducerConfigurations = configuration.GetSection("KafkaSyncProducerConfiguration").Get<KafkaSyncProducerConfiguration>() ?? new KafkaSyncProducerConfiguration();

        var RedisConfigurations = configuration.GetSection("RedisConfiguration").Get<RedisConfiguration>() ?? new RedisConfiguration();

        services.AddSingleton(KafkaSyncProducerConfigurations);
        try
        {
            services.AddSingleton(RedisConfigurations);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return services;
    }

    public static IServiceCollection AddConsumerCachingServices(this IServiceCollection services, IConfiguration configuration)
    {

        var KafkaSyncConsumerConfiguration = configuration.GetSection("KafkaSyncConsumerConfiguration").Get<KafkaSyncConsumerConfiguration>() ?? new KafkaSyncConsumerConfiguration();

        var RedisConfigurations = configuration.GetSection("RedisConfiguration").Get<RedisConfiguration>() ?? new RedisConfiguration();

        services.AddSingleton(KafkaSyncConsumerConfiguration.Default!);

        try
        {
            services.AddSingleton(RedisConfigurations);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return services;
    }

}