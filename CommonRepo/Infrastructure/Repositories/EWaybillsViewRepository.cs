using CommonRepo.Domain.Entities.Waybills;
using CommonRepo.Domain.Interfaces;
using CommonRepo.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CommonRepo.Infrastructure.Repositories;

public class EWaybillsViewRepository : IEWaybillsViewRepository
{
    private readonly ApplicationDbContext _context;

    public EWaybillsViewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EWaybillsView?> GetByIdAsync(int id)
    {
        return await _context.Set<EWaybillsView>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }
}
