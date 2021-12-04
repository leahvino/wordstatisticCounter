using BL.InputData;
using BL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WordStatisticCounterController : ControllerBase
    {
        private readonly IWordStatisticCounter _wordStatisticCounter;
        public WordStatisticCounterController(IWordStatisticCounter wordStatisticCounter)
        {
            _wordStatisticCounter = wordStatisticCounter;
        }
      
        [HttpPost]
        public async Task<IActionResult> SaveWordCounter([FromBody] string buffer)
        {
            _wordStatisticCounter.SetDataType(new StringInputData());
            var response = _wordStatisticCounter.SaveWordCounter();

            if (response is null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> SaveWordCounter([FromBody]  Uri buffer)
        {
            _wordStatisticCounter.SetDataType(new UrlInputData());
            var response = _wordStatisticCounter.SaveWordCounter();

            if (response is null)
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}
