using Configuration.Core.Events;
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
    public class ValuesController : ControllerBase
    {
        private readonly IOptions<AppSettings> config;
        private readonly IConfiguration _configuration;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRedisCacheService _redisCacheManager;
        private readonly string cacheKey = "configurations";

        public ValuesController(IOptions<AppSettings> config, IConfiguration configuration, IPublishEndpoint publishEndpoint, IRedisCacheService redisCacheManager)
        {
            this.config = config;
            this._configuration = configuration;
            this._publishEndpoint = publishEndpoint;
            this._redisCacheManager = redisCacheManager;
        }

        [HttpGet]  
        public async Task<IActionResult> GetValues()
        {
            var appName = this.config.Value.Name;
            List<ConfigurationTb> result;
            var conString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection"); // read logDb connection 
            RequestConfigurationEvent requestConfigurationEvent = new RequestConfigurationEvent()
            {
                ApplicationName = appName,
                ConnectionString = conString,
                RefreshTimerIntervalInMs = 3000 
            };
            await _publishEndpoint.Publish(requestConfigurationEvent);
            try
            {
                result = _redisCacheManager.Get<List<ConfigurationTb>>(cacheKey + $"_{appName}");
            }
            catch (RedisNotAvailableException)
            {
                result = new List<ConfigurationTb>();
            }
            return Ok(result);
        }


        [HttpGet("get_value_by_key")]
        public async Task<IActionResult> GetValueByKey(string key)
        {
            var conString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection"); // read logDb connection 
            //var result = await configurationReader.GetValue<string>(key, conString);
            return Ok();
        }
    }
}
