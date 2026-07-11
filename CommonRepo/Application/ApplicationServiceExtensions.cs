using CommonLibrary.Email;
using CommonLibrary.RequestInformation;
using CommonLibrary.Utilities.FileHelpers;
using CommonRepo.Application.Formatters;
using CommonRepo.Application.Services;
using CommonRepo.Domain.Interfaces;
using CommonRepo.Domain.Interfaces.SystemLogsInterfaces;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommonRepo.Application;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers(options =>
            {
                // Remove the default XmlSerializerOutputFormatter if present
                var defaultXmlFormatter = options.OutputFormatters
                    .OfType<Microsoft.AspNetCore.Mvc.Formatters.XmlSerializerOutputFormatter>()
                    .FirstOrDefault();
                if (defaultXmlFormatter != null)
                    options.OutputFormatters.Remove(defaultXmlFormatter);

                // Add our custom formatter that ignores FilterSearch.NullValue
                options.OutputFormatters.Add(new FilterSearchXmlSerializerOutputFormatter());
            })
            .AddNewtonsoftJson()
            .AddXmlSerializerFormatters();

        // Remove the default formatter added by AddXmlSerializerFormatters and keep only our custom one
        services.PostConfigure<Microsoft.AspNetCore.Mvc.MvcOptions>(options =>
        {
            var defaultFormatters = options.OutputFormatters
                .OfType<Microsoft.AspNetCore.Mvc.Formatters.XmlSerializerOutputFormatter>()
                .Where(f => f.GetType() != typeof(FilterSearchXmlSerializerOutputFormatter))
                .ToList();
            foreach (var formatter in defaultFormatters)
                options.OutputFormatters.Remove(formatter);
        });

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen();

        services.AddHttpContextAccessor();

        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

        services.AddScoped<IRequestInfoService, RequestInfoService>();

        services.AddScoped<ILocalDate, LocalDate>();

        services.AddScoped<IFileUploadUtility, FileUploadUtility>();
        services.AddScoped<IEmailService, EmailService>();


        services.AddLogServices();

        return services;
    }

    public static IServiceCollection AddLogServices(this IServiceCollection services)
    {
        services.AddScoped<IErrorLoggingService, ErrorLoggingService>();

        services.AddScoped(typeof(ILogService<,>), typeof(LogService<,>));

        return services;
    }
}
