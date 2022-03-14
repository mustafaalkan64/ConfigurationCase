using ConfigurationCase.DAL;
using ConfigurationCase.DAL.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace ServiceA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IOptions<AppSettings> config;
        private readonly IConfiguration _configuration;
        private readonly IConfigurationReader configurationReader;
        public ValuesController(IOptions<AppSettings> config, IConfiguration configuration, IConfigurationReader configurationReader)
        {
            this.config = config;
            this._configuration = configuration;
            this.configurationReader = configurationReader; 
        }

        [HttpGet]  
        public async Task<IActionResult> GetValues()
        {
            var appName = this.config.Value.Name;
            var conString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection"); // read logDb connection 
            var result = await configurationReader.ReadConfigurationsAsync(appName, conString, 30);
            return Ok();
        }


        [HttpGet("get_value_by_key")]
        public async Task<IActionResult> GetValueByKey(string key)
        {
            var conString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection"); // read logDb connection 
            var result = await configurationReader.GetValue<string>(key, conString);
            return Ok(result);
        }
    }
}
