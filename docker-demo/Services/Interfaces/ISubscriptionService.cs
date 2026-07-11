using docker_demo.DTOs.SubscriptionDto;

namespace docker_demo.Services.Interfaces
{
    public interface ISubscriptionService
    {
        Task<object> ValidateTruck(SubscriptionDTO subDTO);
    }
}
