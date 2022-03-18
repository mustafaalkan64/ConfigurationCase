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
using Configuration.Core.Consts;
using Configuration.Core.Models;
using AutoMapper;

namespace ConfigurationCase.ConfigurationSource.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IRedisCacheService _redisCacheManager;
        private readonly IMapper _mapper;
        public ConfigurationService(IRedisCacheService redisCacheManager, IMapper mapper)
        {
            _redisCacheManager = redisCacheManager;
            _mapper = mapper;
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
                result = _redisCacheManager.Get<List<ConfigurationTb>>(StaticVariables.CacheKey + $"_{applicationName}");
            }
            catch (RedisNotAvailableException)
            {
                result = new List<ConfigurationTb>();
            }
            return result;
            
        }

        public async Task AddNewRecord(ConfigurationDto configurationDto, string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(connectionString);

            using (var db = new ConfigurationDbContext(optionsBuilder.Options))
            {
                var configuration = _mapper.Map<ConfigurationTb>(configurationDto);
                await db.Configuration.AddAsync(configuration);
                await db.SaveChangesAsync();
            }

        }


        public async Task UpdateRecord(UpdateConfigurationDto configurationDto, string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(connectionString);

            using (var db = new ConfigurationDbContext(optionsBuilder.Options))
            {
                var configuration = await db.Configuration.AsNoTracking().FirstOrDefaultAsync(x => x.ID == configurationDto.ID);
                if(configuration == null)
                {
                    throw new ArgumentException("ID Bulunamadı");
                }
                var updatedConfiguration = _mapper.Map<ConfigurationTb>(configurationDto);
                db.Entry(updatedConfiguration).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }

        }


        public async Task RemoveRecord(int Id, string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(connectionString);

            using (var db = new ConfigurationDbContext(optionsBuilder.Options))
            {
                var configuration = await db.Configuration.AsNoTracking().FirstOrDefaultAsync(x => x.ID == Id);
                if (configuration == null)
                {
                    throw new ArgumentException("ID Bulunamadı");
                }
                db.Configuration.Attach(configuration);
                db.Configuration.Remove(configuration);
                await db.SaveChangesAsync();
            }

        }

    }
}
