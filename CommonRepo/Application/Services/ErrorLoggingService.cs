using CommonLibrary.RequestInformation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using CommonRepo.Domain.Entities;
using CommonRepo.Domain.Interfaces;

namespace CommonRepo.Application.Services
{
    public class ErrorLoggingService : IErrorLoggingService
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<SystemErrorsLog> _dbSet;
        private readonly IRequestInfoService _requestInfoService;
        private readonly IConfiguration _configuration;

        public ErrorLoggingService(DbContext errorsLogDbContext, IRequestInfoService requestInfoService, IConfiguration configuration)
        {
            _dbContext = errorsLogDbContext;
            _dbSet = _dbContext.Set<SystemErrorsLog>();
            _requestInfoService = requestInfoService;
            _configuration = configuration;
        }

        public async Task<SystemErrorsLog> CreateSystemErrorLogAsync(SystemErrorsLogDTO errorLogDTO)
        {
            var requestInfo = _requestInfoService.GetRequestInfo();

            if (errorLogDTO.InnerEx.InnerException != null)
            {
                errorLogDTO.InnerEx = errorLogDTO.InnerEx.InnerException;
            }

            string innerErrorDetails = errorLogDTO.InnerEx!.Message;

            string innerErrorTagContext = errorLogDTO.InnerEx!.StackTrace;

            string jsonArgs = JsonConvert.SerializeObject(errorLogDTO.Args);

            SystemErrorsLog systemErrorsLog = new()
            {
                ErrorDetails = innerErrorDetails,
                ErrorTagContext = innerErrorTagContext,
                Args = jsonArgs,
                ControllerName = errorLogDTO.ControllerName,
                RequestAPI = errorLogDTO.RequestAPI,
                LocalIpAddress = requestInfo.ServerIpAddress,
                RemoteIpAddress = requestInfo.ClientIpAddress,
                UserId = requestInfo.UserId!,
                Browser = requestInfo.UserAgent,
                ErrorSource = requestInfo.Source
            };

            try
            {
                await _dbSet.AddAsync(systemErrorsLog);

                await _dbContext.SaveChangesAsync();
            }

            catch (Exception outerEx)
            {
                string outerErrorDetails = "";

                string outerErrorTagContext = "";

                if (outerEx.InnerException != null)
                {
                    outerErrorDetails = outerEx.InnerException.Message;

                    outerErrorTagContext = outerEx.InnerException.StackTrace!;
                }
                else
                {
                    outerErrorDetails = outerEx.Message;

                    outerErrorTagContext = outerEx.StackTrace!;
                }

                SystemErrorsLogFile systemErrorsLogFile = new()
                {
                    InnerErrorDetails = innerErrorDetails,
                    InnerErrorTagContext = innerErrorTagContext,
                    OuterErrorDetails = outerErrorDetails,
                    OuterErrorTagContext = outerErrorTagContext,
                    Args = jsonArgs,
                    ControllerName = errorLogDTO.ControllerName,
                    RequestAPI = errorLogDTO.RequestAPI,
                    LocalIpAddress = requestInfo.ServerIpAddress,
                    RemoteIpAddress = requestInfo.ClientIpAddress,
                    UserId = requestInfo.UserId!,
                    Browser = requestInfo.UserAgent,
                    ErrorSource = requestInfo.Source

                };

                var folderPath = _configuration["ErrorsFolderPath"]!;

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var filePath = folderPath + "/" + DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss") + ".json";

                string json = JsonConvert.SerializeObject(systemErrorsLogFile, Formatting.Indented);
                System.IO.File.WriteAllText(filePath, json);
            }

            return systemErrorsLog;

        }
    }
}
