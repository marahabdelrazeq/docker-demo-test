using CommonRepo.Domain.Interfaces.SubscritionsIRepos;
using docker_demo.DTOs.SubscriptionDto;
using docker_demo.Services.Interfaces;

namespace docker_demo.Services.Implementations
{
    public class SubscriptionService: ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionService(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }
        public async Task<object> ValidateTruck(SubscriptionDTO subDTO)
        {

            var truckInfo = await _subscriptionRepository.GetAsync(s => s.plate_number == subDTO.PlateNumber && s.plate_code_en== subDTO.PlateCodeEn);

            if (truckInfo is not null)
                return new { Status = "AUTHORIZED" };

            return new { Status = "NOT_AUTHORIZED" };
        }
    }
}
