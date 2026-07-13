using CommonRepo.Domain.Entities.Waybills;

namespace CommonRepo.Domain.Interfaces;

public interface IEWaybillsViewRepository
{
    Task<EWaybillsView?> GetByIdAsync(int id);
}
