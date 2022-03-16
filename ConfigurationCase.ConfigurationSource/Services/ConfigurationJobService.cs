using ConfigurationCase.ConfigurationSource.Abstracts;
using ConfigurationCase.Core.Entities;
using ConfigurationCase.ConfigurationSource.Jobs;
using ConfigurationCase.Core.Caching;
using Hangfire;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationCase.ConfigurationSource.Services
{
    public class ConfigurationJobService : IConfigurationJobService
    {

        public async Task ReadConfigurationAsync(string applicationName, string connectionString, int refreshTimerIntervalInMs)
        {
            var minutes = Convert.ToInt32(TimeSpan.FromMilliseconds(refreshTimerIntervalInMs).TotalMinutes);
            Hangfire.RecurringJob.AddOrUpdate<ConfigurationReaderJob>(job => job.GetConfigurationsAsync(applicationName, connectionString), cronExpression: $"*/{ (minutes > 0 ? minutes : 1) } * * * *");
        }

    }
}
