namespace CommonRepo.Domain.Interfaces;

public interface ILocalDate
{
    public Task<DateTime> GetLocalTime(string countryCode);
}
