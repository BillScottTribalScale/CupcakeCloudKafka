using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KafkaListener.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KafkaListener.Api.Controllers 
{
    [ExcludeFromCodeCoverage]
    [Route ("api/[controller]")]
    [ApiController]
    public class KafkaListenerController : ControllerBase 
    {
        private readonly ILogger<KafkaListenerController> _logger;

        public KafkaListenerController (ILogger<KafkaListenerController> logger) 
        {
           _logger = logger; 
        }

        [HttpPost]
        [Route ("Start")]
        public IActionResult Start()
        {                      
            return new OkResult();
        }

        [HttpPost]
        [Route ("Stop")]
        public IActionResult Stop() 
        {            
            return new OkResult();           
        }
    }
}
