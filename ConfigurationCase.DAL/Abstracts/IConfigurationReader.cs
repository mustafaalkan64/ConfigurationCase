using ConfigurationCase.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationCase.DAL.Abstracts
{
    public interface IConfigurationReader
    {
        Task<List<Configuration>> ReadConfigurationsAsync(string applicationName, string connectionString, int refreshTimerIntervalInMs);
        Task<T> GetValue<T>(string key, string connectionString);
    }
}
