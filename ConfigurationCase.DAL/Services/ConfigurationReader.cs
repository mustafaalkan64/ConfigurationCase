using ConfigurationCase.DAL.Abstracts;
using ConfigurationCase.DAL.Entities;
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
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(connectionString);

            using (var db = new ConfigurationDbContext(optionsBuilder.Options))
            {
                var records = await db.Configuration.Where(x => x.ApplicationName == applicationName && x.IsActive).ToListAsync();
                return records;
            }
        }

        
    }
}
