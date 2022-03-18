using Configuration.Core.Events;
using Configuration.Core.Models;
using ConfigurationCase.ConfigurationSource.Abstracts;
using ConfigurationCase.Core;
using ConfigurationCase.Core.Caching;
using ConfigurationCase.Core.CustomExceptions;
using ConfigurationCase.Core.Entities;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly IOptions<AppSettings> config;
        private readonly IConfiguration _configuration;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IConfigurationService _configurationService;
        private readonly string conString;
        private readonly string appName;

        public ConfigurationController(IOptions<AppSettings> config, IConfiguration configuration, IPublishEndpoint publishEndpoint, IConfigurationService configurationService)
        {
            this.config = config;
            this._configuration = configuration;
            this._publishEndpoint = publishEndpoint;
            this._configurationService = configurationService;
            conString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
            appName = this.config.Value.Name;
        }

        [HttpGet]  
        public async Task<IActionResult> GetValues()
        {
            var appName = this.config.Value.Name;
            IList<ConfigurationTb> result;
            //var conString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection"); // read logDb connection 
            result = await _configurationService.GetConfigurationsAsync(appName);
            return Ok(result);
        }


        [HttpGet]
        [Route("execute_read_configuration_job")]
        public async Task<IActionResult> ExecuteJob(int miliSecond)
        {
            var appName = this.config.Value.Name;
            RequestConfigurationEvent requestConfigurationEvent = new RequestConfigurationEvent()
            {
                ApplicationName = appName,
                ConnectionString = conString,
                RefreshTimerIntervalInMs = miliSecond
            };
            await _publishEndpoint.Publish(requestConfigurationEvent);
            return NoContent();
        }


        [HttpGet("get_value_by_key")]
        public async Task<IActionResult> GetValueByKey(string key)
        {
            var result = await _configurationService.GetValue<string>(key, conString, appName);
            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> PostConfiguration(AddConfigurationDto addConfigurationDto)
        {
            ConfigurationDto configurationDto = new ConfigurationDto()
            {
                Name = addConfigurationDto.Name,
                Value = addConfigurationDto.Value,
                ApplicationName = appName,
                IsActive = addConfigurationDto.IsActive,
                Type = addConfigurationDto.Type
            };

            await _configurationService.AddNewRecord(configurationDto, conString);
            return NoContent();
        }


        [HttpPut]
        public async Task<IActionResult> UpdateConfiguration(UpdateConfigurationDto updateConfigurationDto)
        {
            updateConfigurationDto.ApplicationName = appName;
            await _configurationService.UpdateRecord(updateConfigurationDto, conString);
            return NoContent();
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteConfiguration(int id)
        {
            await _configurationService.RemoveRecord(id, conString);
            return NoContent();
        }
    }
}
