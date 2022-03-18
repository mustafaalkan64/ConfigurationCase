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

        public async Task AddNewRecord(ConfigurationDto configurationDto)
        {
            var configuration = _mapper.Map<ConfigurationTb>(configurationDto);
            await this.dbContext.Configuration.AddAsync(configuration);
            await this.dbContext.SaveChangesAsync();

        }


        public async Task UpdateRecord(UpdateConfigurationDto configurationDto)
        {
            var configuration = await this.dbContext.Configuration.AsNoTracking().FirstOrDefaultAsync(x => x.ID == configurationDto.ID);
            if (configuration == null)
            {
                throw new ArgumentException("ID Bulunamadı");
            }
            var updatedConfiguration = _mapper.Map<ConfigurationTb>(configurationDto);
            this.dbContext.Entry(updatedConfiguration).State = EntityState.Modified;
            await this.dbContext.SaveChangesAsync();

        }


        public async Task RemoveRecord(int Id)
        {
            var configuration = await this.dbContext.Configuration.AsNoTracking().FirstOrDefaultAsync(x => x.ID == Id);
            if (configuration == null)
            {
                throw new ArgumentException("ID Bulunamadı");
            }
            this.dbContext.Configuration.Attach(configuration);
            this.dbContext.Configuration.Remove(configuration);
            await this.dbContext.SaveChangesAsync();

        }


        public async Task<IEnumerable<ConfigurationDto>> GetRecordsByTerm(string term)
        {
            var result = await this.dbContext.Configuration.AsNoTracking().Where(x => x.Name.Contains(term) || x.Value.Contains(term)).ToListAsync();
            var configurations = _mapper.Map<IEnumerable<ConfigurationDto>>(result);
            return configurations;

        }

    }
}
