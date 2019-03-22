using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbrantosAPI.Authentication;
using AbrantosAPI.Data;
using AbrantosAPI.Models.Register;
using AbrantosAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AbrantosAPI.Controllers
{
    [Authorize("Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class DailyRegisterController : ControllerBase
    {
        private readonly AbrantosContext _context;
        private readonly IDailyRegisterService _dailyRegisterService;
        private readonly IHttpContextAccessor _httpContext;
        public DailyRegisterController(AbrantosContext context,
                                    IDailyRegisterService dailyRegisterService,
                                    IHttpContextAccessor httpContext)
        {
            _context = context;
            _dailyRegisterService = dailyRegisterService;
            _httpContext = httpContext;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDailyRegisterViewModel dailyRegister)
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            DailyRegister mappedDailyRegister = new DailyRegister(dailyRegister.Abrantos, dailyRegister.Date, userId);

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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDailyRegisterViewModel dailyRegister)
        {
            DailyRegister mappedDailyRegister = new DailyRegister(dailyRegister.Abrantos, dailyRegister.Date, "");
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