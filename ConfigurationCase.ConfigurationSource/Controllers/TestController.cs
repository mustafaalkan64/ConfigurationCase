using ConfigurationCase.ConfigurationSource.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ConfigurationCase.ConfigurationSource.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IConfigurationReaderService _service;
        public TestController(IConfigurationReaderService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetValues()
        {
            await _service.ReadConfigurationAsync("ssdfsdf", "Server=WSMALKAN;Database=ConfigurationDB;Trusted_Connection=True;MultipleActiveResultSets=true", 3000);
            return Ok();
        }
    }
}
