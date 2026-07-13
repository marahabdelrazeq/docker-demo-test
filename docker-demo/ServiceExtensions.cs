 
using CommonRepo.Domain.Entities;
using CommonRepo.Domain.Interfaces;
using CommonRepo.Infrastructure.Caching;
using docker_demo.Services.Implementations;
using docker_demo.Services.Interfaces;
using Microsoft.AspNetCore.Server.Kestrel.Core;
namespace docker_demo
{
    public static class ServiceExtensions
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        { 
            services.AddHttpClient(); 
            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = 40 * 1024 * 1024; // 40 MB
            });
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddSingleton<IWaybillPdfService, WaybillPdfService>();
            services.AddScoped<IWaybillReportService, WaybillReportService>();

            // Register Quartz scheduler with CleanupPendingStakeholdersJob

        }
    }
}
