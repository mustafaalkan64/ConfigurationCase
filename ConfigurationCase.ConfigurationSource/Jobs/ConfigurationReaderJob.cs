using ConfigurationCase.Core.Entities;
using ConfigurationCase.Core.Caching;
using ConfigurationCase.Core.CustomExceptions;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configuration.Core.Consts;

namespace ConfigurationCase.ConfigurationSource.Jobs
{
    public class ConfigurationReaderJob
    {
        private readonly IRedisCacheService _redisCacheManager;
        public ConfigurationReaderJob(IRedisCacheService redisCacheManager)
        {
            _redisCacheManager = redisCacheManager;
        }

        [JobDisplayName("GetConfigurationsAsync")]
        [AutomaticRetry(Attempts = 3)]
        public async Task GetConfigurationsAsync(string applicationName, string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(connectionString);

            using (var db = new ConfigurationDbContext(optionsBuilder.Options))
            {
                var records = await db.Configuration.AsNoTracking().Where(x => x.ApplicationName == applicationName && x.IsActive).ToListAsync();
                _redisCacheManager.Set(StaticVariables.CacheKey + $"_{applicationName}", records);
            }

        }
    }
}
