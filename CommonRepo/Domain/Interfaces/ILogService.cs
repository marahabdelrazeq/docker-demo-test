namespace CommonRepo.Domain.Interfaces.SystemLogsInterfaces;

public interface ILogService<TLog, TView> where TLog : class, new() where TView : class, new()
{
    Task<bool> LogTransaction(TView originalView, TView newView, string actionName = null, int? userId = null);

    Task<bool> LogTransaction<TEntity>(dynamic originalEntity, TEntity newEntity, string actionName = null, int? userId = null) where TEntity : class, new();

    Task<TEntity> LogTransaction<TEntity>(TEntity originalEntity, Func<Task<TEntity>> ActionFunc, string actionName = null, int? userId = null) where TEntity : class, new();
}
