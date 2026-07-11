using CommonRepo.Persistence.Context;
using CommonRepo.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommonRepo.Persistence
{
    public static class PersistenceServiceExtensions
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            var defaultSQLConnection = configuration.GetConnectionString("DefaultSQLConnection");

            services.AddDbContext<DbContext, ApplicationDbContext>(option =>
            {
                option.UseSqlServer(defaultSQLConnection,
                    sqlOptions => sqlOptions.UseNetTopologySuite());

                //option.UseSqlServer(defaultSQLConnection);
                option.EnableSensitiveDataLogging();

            });

            services.AddDbContext<DbContext, SystemErrorsLogDbContext>(option =>
            {
                option.UseSqlServer(defaultSQLConnection);
            });

            return services;
        }
    }
}
