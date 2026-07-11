using CommonLibrary.RequestInformation;
using CommonRepo.Domain.Entities.Subscriptions;
using CommonRepo.Domain.Interfaces.SubscritionsIRepos;
using CommonRepo.Persistence.Context;
using CommonRepo.Domain.Interfaces;
using CommonRepo.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonRepo.Infrastructure.Repositories.SubscriptionRepos
{
    public class SubscriptionRepository : Repository<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(ApplicationDbContext context, IRequestInfoService requestInfoService, string failureMessage = null) : base(context, requestInfoService, failureMessage)
        {
        }
    }
}
