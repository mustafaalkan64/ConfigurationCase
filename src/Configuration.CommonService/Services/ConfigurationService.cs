using ConfigurationCase.ConfigurationSource.Abstracts;
using ConfigurationCase.Core.Entities;
using ConfigurationCase.Core.Caching;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigurationCase.Core.CustomExceptions;
using Configuration.Core.Consts;
using Configuration.Core.Models;
using AutoMapper;
using ConfigurationCase.DAL;

namespace ConfigurationCase.ConfigurationSource.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IRedisCacheService _redisCacheManager;
        private readonly IMapper _mapper;
        private readonly ConfigurationDbContext dbContext;
        public ConfigurationService(IRedisCacheService redisCacheManager, IMapper mapper, ConfigurationDbContext dbContext)
        {
            _redisCacheManager = redisCacheManager;
            _mapper = mapper;
            this.dbContext = dbContext;
        }

        public async Task<T> GetValue<T>(string key, string appName)
        {
            var typeName = typeof(T).Name;
            var record = await dbContext.Configuration.Where(x => x.Name == key && x.ApplicationName == appName).FirstOrDefaultAsync();
            return (T)Convert.ChangeType(record?.Value ?? string.Empty, typeof(T));
        }

        public async Task<IEnumerable<ConfigurationDto>> GetConfigurationsAsync(string applicationName)
        {
            IEnumerable<ConfigurationDto> result;
            try
            {
                result = _redisCacheManager.Get<IEnumerable<ConfigurationDto>>(StaticVariables.CacheKey + $"_{applicationName}");
            }
            catch (RedisNotAvailableException)
            {
                result = new List<ConfigurationDto>();
            }
            return result;

        }

        public async Task AddNewRecord(ConfigurationDto configurationDto)
        {
            var configuration = _mapper.Map<Core.Entities.Configuration>(configurationDto);
            await this.dbContext.Configuration.AddAsync(configuration);
            await this.dbContext.SaveChangesAsync();

        }


        public async Task UpdateRecord(UpdateConfigurationDto configurationDto)
        {
            var configuration = await this.dbContext.Configuration.AsNoTracking().FirstOrDefaultAsync(x => x.ID == configurationDto.ID && x.ApplicationName == configurationDto.ApplicationName);
            if (configuration == null)
            {
                throw new ArgumentException("ID Bulunamadı");
            }
            var updatedConfiguration = _mapper.Map<Core.Entities.Configuration>(configurationDto);
            this.dbContext.Entry(updatedConfiguration).State = EntityState.Modified;
            await this.dbContext.SaveChangesAsync();
            this.dbContext.Entry(updatedConfiguration).State = EntityState.Detached;

        }


        public async Task RemoveRecord(int Id, string appName)
        {
            var configuration = await this.dbContext.Configuration.FirstOrDefaultAsync(x => x.ID == Id && x.ApplicationName == appName);
            if (configuration == null)
            {
                throw new ArgumentException("ID Bulunamadı");
            }
            this.dbContext.Configuration.Attach(configuration);
            this.dbContext.Configuration.Remove(configuration);
            await this.dbContext.SaveChangesAsync();

        }


        public async Task<IEnumerable<ConfigurationDto>> GetRecordsByTerm(string term, string appName)
        {
            var result = await this.dbContext.Configuration.Where(x => (x.Name.Contains(term) || x.Value.Contains(term)) && x.ApplicationName == appName).ToListAsync();
            var configurations = _mapper.Map<IEnumerable<ConfigurationDto>>(result);
            return configurations;

        }

    }
}
