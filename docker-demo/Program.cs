using CommonRepo.Application;
using CommonRepo.Application.Middlewares;
using CommonRepo.Infrastructure;
using CommonRepo.Persistence;
using docker_demo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
 


ConfigureHost(builder.Host);

// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.RegisterServices(builder.Configuration);




var app = builder.Build();


app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsEnvironment("Dev") || app.Environment.IsEnvironment("Local"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
 
 

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();

void ConfigureHost(IHostBuilder hostBuilder)
{
    hostBuilder.ConfigureAppConfiguration((hostingContext, config) =>
    {
        var env = hostingContext.HostingEnvironment;

        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

        Console.WriteLine("***********************************************");

        Console.WriteLine(env.EnvironmentName);

        config.AddEnvironmentVariables();

        Console.WriteLine("***********************************************");
    });
}
