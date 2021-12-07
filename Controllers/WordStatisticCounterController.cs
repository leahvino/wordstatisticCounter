using BL.InputData;
using BL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Controllers
{
    //[Route("api/[controller]/[action]/{?id}")]
    ////[ApiController]
    public class WordStatisticCounterController : ControllerBase
    {
        private readonly IWordCounterBO _wordCounterBO;
        public WordStatisticCounterController(IWordCounterBO wordCounterBO)
        {
            _wordCounterBO = wordCounterBO;
        }

        [HttpGet]
        public async Task<string> HealthCheck()
        {
            return  "Success";
        }

        [HttpPost]
        public async Task<IActionResult> SaveWordCounter([FromBody] WordcounterRequest buffer)
        {
            //_wordCounter.SetDataType(new StringInputData());
            var response = await _wordCounterBO.SaveWordCounter(buffer.Buffer);

            if (response != null)
            {
                return Ok(response);                
            }
            return NotFound();

        }

        [HttpPost]
        public async Task<IActionResult> GetWordStatistic(string wordName )
        {
            var response = await _wordCounterBO.SaveWordCounter();

            if (response is null)
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}
