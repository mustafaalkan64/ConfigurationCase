using ConfigurationCase.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationCase.ConfigurationSource.Abstracts
{
    public interface IConfigurationReaderService
    {
        Task ReadConfigurationAsync(string applicationName, string connectionString, int refreshTimerIntervalInMs);
        Task<T> GetValue<T>(string key, string connectionString);
    }
}
