using ConfigurationCase.ConfigurationSource.Abstracts;
using ConfigurationCase.Core.Entities;
using ConfigurationCase.Core.Caching;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigurationCase.CommonService;
using ConfigurationCase.Core.CustomExceptions;

namespace ConfigurationCase.ConfigurationSource.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IRedisCacheService _redisCacheManager;
        private readonly string cacheKey = "configurations";
        public ConfigurationService(IRedisCacheService redisCacheManager)
        {
            _redisCacheManager = redisCacheManager;
        }

        public async Task<T> GetValue<T>(string key, string connectionString, string appName)
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(connectionString);
            var typeName = typeof(T).Name;

            using (var db = new ConfigurationDbContext(optionsBuilder.Options))
            {
                var record = await db.Configuration.Where(x => x.Name == key && x.Type.ToUpper() == typeName.ToUpper() && x.ApplicationName == appName).FirstOrDefaultAsync();
                return (T)Convert.ChangeType(record.Value, typeof(T));
            }
        }

        public async Task<IList<ConfigurationTb>> GetConfigurationsAsync(string applicationName)
        {
            IList<ConfigurationTb> result;
            try
            {
                result = _redisCacheManager.Get<List<ConfigurationTb>>(cacheKey + $"_{applicationName}");
            }
            catch (RedisNotAvailableException)
            {
                result = new List<ConfigurationTb>();
            }
            return result;
            
        }

    }
}
