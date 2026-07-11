using CommonRepo.Domain.Entities;

namespace CommonRepo.Domain.Interfaces;

public interface IErrorLoggingService
{
    Task<SystemErrorsLog> CreateSystemErrorLogAsync(SystemErrorsLogDTO errorLogDTO);
}
