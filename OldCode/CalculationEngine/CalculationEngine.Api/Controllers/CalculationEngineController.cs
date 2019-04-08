using Microsoft.AspNetCore.Mvc;
using CalculationEngine.Api.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CalculationEngine.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CalculationEngineController : ControllerBase
    {
        private readonly ICalculationEngineService _calcEngine;
        private readonly ILogger<CalculationEngineController> _logger;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="calcEngine"></param>
        public CalculationEngineController(ILogger<CalculationEngineController> logger, ICalculationEngineService calcEngine)
        {
            _logger = logger;
            _calcEngine = calcEngine;
        }

        // POST api/values
        [HttpPost("process")]
        public IActionResult Process([FromBody] string messageContent)
        {
            if (!string.IsNullOrEmpty(messageContent) && _calcEngine.StartProcess(messageContent))
            {
                return new OkResult();
            }
            return new BadRequestResult();
        }
    }
}
