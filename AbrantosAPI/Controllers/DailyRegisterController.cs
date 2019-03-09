using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbrantosAPI.Data;
using AbrantosAPI.Models.Register;
using AbrantosAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AbrantosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyRegisterController : ControllerBase
    {
        private readonly AbrantosContext _context;
        private readonly IDailyRegisterService _dailyRegisterService;
        public DailyRegisterController(AbrantosContext context,
                                    IDailyRegisterService dailyRegisterService)
        {
            _context = context;
            _dailyRegisterService = dailyRegisterService;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            try 
            {
                var dailyRegisterList = await _dailyRegisterService.Get();

                return StatusCode(200, new
                {
                    dailyRegisterList
                });
            }
            catch(NullReferenceException e)
            {
                return StatusCode(404, e.Message);
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try 
            {
                var dailyRegister = await _dailyRegisterService.Get(id);

                return StatusCode(200, new
                {
                    dailyRegister
                });
            }
            catch(KeyNotFoundException e)
            {
                return StatusCode(404, e.Message);
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateDailyRegisterViewModel dailyRegister)
        {
            DailyRegister mappedDailyRegister = new DailyRegister(dailyRegister.Abrantos, dailyRegister.Date);

            try
            {
                var result = await _dailyRegisterService.Create(mappedDailyRegister);
                return StatusCode(200, new
                {
                    result
                });
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateDailyRegisterViewModel dailyRegister)
        {
            DailyRegister mappedDailyRegister = new DailyRegister(dailyRegister.Abrantos, dailyRegister.Date);
            mappedDailyRegister.Id = id;

            try
            {
                var result = await _dailyRegisterService.Update(mappedDailyRegister);
                return StatusCode(200, new
                {
                    result
                });
            }
            catch(KeyNotFoundException e)
            {
                return StatusCode(404, e.Message);
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _dailyRegisterService.Delete(id);
                return Ok();
            }
            catch(KeyNotFoundException e)
            {
                return StatusCode(404, e.Message);
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}