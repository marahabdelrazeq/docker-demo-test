using CommonLibrary.RequestInformation;
using CommonRepo.Domain.Entities;
using CommonRepo.Domain.Interfaces;
using CommonRepo.Infrastructure.Repositories;
using CommonRepo.Persistence.Context;

namespace CommonRepo.Infrastructure.Repositories
{
    public class SystemConfigurationRepository : Repository<SystemConfiguration>, ISystemConfigurationRepository
    {
        public SystemConfigurationRepository (ApplicationDbContext context, IRequestInfoService requestInfoService) : base(context, requestInfoService)
        {

        }
    }
}
