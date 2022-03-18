using Configuration.Core.Models;
using ConfigurationCase.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationCase.ConfigurationSource.Abstracts
{
    public interface IConfigurationService
    {
        Task<IList<ConfigurationTb>> GetConfigurationsAsync(string applicationName);
        Task<T> GetValue<T>(string key, string appName);
        Task AddNewRecord(ConfigurationDto configurationDto);
        Task UpdateRecord(UpdateConfigurationDto configurationDto);
        Task RemoveRecord(int Id);
    }
}
