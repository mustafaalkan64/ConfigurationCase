using ConfigurationCase.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationCase.ConfigurationSource.Abstracts
{
    public interface IConfigurationJobService
    {
        Task ReadConfigurationAsync(string applicationName, string connectionString, int refreshTimerIntervalInMs);
    }
}
