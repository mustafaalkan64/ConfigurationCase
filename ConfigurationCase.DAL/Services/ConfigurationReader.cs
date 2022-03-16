using ConfigurationCase.Core.Caching;
using ConfigurationCase.DAL.Abstracts;
using ConfigurationCase.DAL.Entities;
using ConfigurationCase.DAL.Jobs;
using Hangfire;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationCase.DAL.Services
{
    public class ConfigurationReader : IConfigurationReader
    {
        private readonly IRedisCacheService _redisCacheManager;
        private readonly string cacheKey = "configurations";
        public ConfigurationReader(IRedisCacheService redisCacheManager)
        {
            _redisCacheManager = redisCacheManager;
        }

        public async Task<T> GetValue<T>(string key, string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(connectionString);
            var typeName = typeof(T).Name;

            using (var db = new ConfigurationDbContext(optionsBuilder.Options))
            {
                var record = await db.Configuration.Where(x => x.Name == key && x.Type.ToUpper() == typeName.ToUpper()).FirstOrDefaultAsync();
                return (T)Convert.ChangeType(record.Value, typeof(T));
            }
        }

        public async Task<List<Configuration>> ReadConfigurationsAsync(string applicationName, string connectionString, int refreshTimerIntervalInMs)
        {
            var minutes = Convert.ToInt32(TimeSpan.FromMilliseconds(refreshTimerIntervalInMs).TotalMinutes);
            Hangfire.RecurringJob.AddOrUpdate<ConfigurationReaderJob>(job => job.GetConfigurationsAsync(applicationName, connectionString), cronExpression: $"*/{ (minutes > 0 ? minutes : 1) } * * * *");
            var cacheList = _redisCacheManager.Get<List<Configuration>>(cacheKey);
            return cacheList;
        }

    }
}
