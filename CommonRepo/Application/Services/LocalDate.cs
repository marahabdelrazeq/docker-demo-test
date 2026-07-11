using CommonLibrary.Caching.Redis;
using CommonRepo.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CommonRepo.Application.Services
{
    public class LocalDate : ILocalDate
    {
        private readonly RedisCacheEngine _cachingEngine;

        private readonly IConfiguration _configuration;

        public LocalDate(RedisCacheEngine cachingEngine, IConfiguration configuration)
        {
            _cachingEngine = cachingEngine;

            _configuration = configuration;
        }

        public async Task<DateTime> GetLocalTime(string countryCode)
        {
            return await ConvertGMTToLocalTime(countryCode);

        }

        #region Private Functions
        private async Task<DateTime> ConvertGMTToLocalTime(string countryCode)
        {
            string timeZone = await GetTimeZone(countryCode);
            // Remove escaped backslashes
            timeZone = timeZone.Replace("\\", "");

            // Remove escaped quotes
            timeZone = timeZone.Replace("\"", "");

            // Replace Unicode escape sequences with corresponding characters
            if (timeZone.StartsWith("u002B"))
            {
                timeZone = timeZone.Replace("u002B", "+");
            }
            else if (timeZone.StartsWith("u002D"))
            {
                timeZone = timeZone.Replace("u002D", "-");
            }

            string[] parts = timeZone.Split(new char[] { '+', ':', '-' });

            string sign = timeZone[0] == '-' ? "-" : "+";

            string hours = parts[1];

            string minutes = parts[2];

            int m = int.Parse(minutes);

            int h = int.Parse(hours);

            int totalMinutes = h * 60 + m;

            if (sign == "-")
            {
                return DateTime.Now - TimeSpan.FromMinutes(totalMinutes);
            }
            else
            {
                return DateTime.Now + TimeSpan.FromMinutes(totalMinutes);
            }

        }

        private async Task<string> GetTimeZone(string countryCode)
        {
            int DbNumber = int.Parse(_configuration["SystemConfiguration:DbCachingNumber"]?.ToString() ?? "");

            string key = $"SystemConfiguration_{countryCode}_Timezone";

            if (await _cachingEngine.KeyExistsAsync(key, DbNumber))
            {
                return await _cachingEngine.GetDataAsync(key, DbNumber);

            }

            return "+02:00";
        }
        #endregion

    }

}

