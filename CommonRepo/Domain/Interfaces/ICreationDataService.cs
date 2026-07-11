namespace CommonRepo.Domain.Interfaces
{
    public interface ICreationDataService<TEntity> where TEntity : class
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}