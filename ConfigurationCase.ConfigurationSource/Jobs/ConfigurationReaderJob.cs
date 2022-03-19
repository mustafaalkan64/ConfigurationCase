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
using ConfigurationCase.DAL;

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

            var contextOptions = new DbContextOptionsBuilder<ConfigurationDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            using (var db = new ConfigurationDbContext(contextOptions))
            {
                var records = await db.Configuration.Where(x => x.ApplicationName == applicationName && x.IsActive).ToListAsync();
                _redisCacheManager.Set(StaticVariables.CacheKey + $"_{applicationName}", records);
            }

        }
    }
}
