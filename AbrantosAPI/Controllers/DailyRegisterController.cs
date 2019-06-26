using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbrantosAPI.Authentication;
using AbrantosAPI.Data;
using AbrantosAPI.Models.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AbrantosAPI.Controllers
{
    [Authorize("Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class DailyRegisterController : ControllerBase
    {
        private readonly AbrantosContext _context;
        private readonly IHttpContextAccessor _httpContext;
        public DailyRegisterController(AbrantosContext context,
                                    IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            try 
            {
                var dailyRegisterList = await _context.DailyRegister.Where(e => e.UserId == userId)
                                                                    .ToListAsync();

                if (dailyRegisterList == null)
                    return NotFound();

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
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            try 
            {
                var dailyRegister = await _context.DailyRegister.FirstOrDefaultAsync(e => (e.Id == id) &&
                                                                                    (e.UserId == userId));

                if (dailyRegister == null)
                    return NotFound();

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

        [HttpGet("date/{date}")]
        public async Task<IActionResult> GetByDate(string date)
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            try 
            {
                var mappedDate = DateTime.Parse(date).ToShortDateString();
                var dailyRegister = await _context.DailyRegister.FirstOrDefaultAsync(e => (e.Date.ToShortDateString() == mappedDate) &&
                                                                                    (e.UserId == userId));

                if (dailyRegister == null)
                    return NotFound();

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
            DailyRegister mappedDailyRegister = new DailyRegister(dailyRegister.Abrantos,
                                                                    dailyRegister.Date,
                                                                    userId,
                                                                    dailyRegister.Post);

            try
            {
                var alreadyRegisteredToday = await _context.DailyRegister
                                                                    .Where(d => d.Date.Day == mappedDailyRegister.Date.Day &&
                                                                        (d.UserId == userId)).AnyAsync();

                if (alreadyRegisteredToday)
                    return StatusCode(400, "Você já registrou abrantos hoje");

                if ((mappedDailyRegister.Abrantos > 1000) || (mappedDailyRegister.Abrantos < -1000))
                    return StatusCode(400, "Abrantos só vão de -1000 a 1000");

                if (mappedDailyRegister.Post.Length > 140)
                    return StatusCode(400, "Um post pode ter no máximo 140 caracteres");

                _context.DailyRegister.Add(mappedDailyRegister);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDailyRegisterViewModel dailyRegister)
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            DailyRegister mappedDailyRegister = new DailyRegister(dailyRegister.Abrantos,
                                                                    dailyRegister.Date,
                                                                    "",
                                                                    dailyRegister.Post);
            mappedDailyRegister.Id = id;

            try
            {
                var oldRegister = await _context.DailyRegister.FirstOrDefaultAsync(e => (e.Id == id) &&
                                                                                    (e.UserId == userId));
                if (dailyRegister == null)
                    return NotFound();

                if ((mappedDailyRegister.Abrantos > 1000) || (mappedDailyRegister.Abrantos < -1000))
                    return StatusCode(400, "Abrantos só vão de -1000 a 1000");

                if (mappedDailyRegister.Post.Length > 140)
                    return StatusCode(400, "Um post pode ter no máximo 140 caracteres");

                oldRegister.Abrantos = mappedDailyRegister.Abrantos;
                oldRegister.Date = mappedDailyRegister.Date;
                oldRegister.Post = mappedDailyRegister.Post;
                _context.DailyRegister.Update(oldRegister);
                await _context.SaveChangesAsync();

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            try
            {
                var oldRegister = await _context.DailyRegister.FirstOrDefaultAsync(e => (e.Id == id) &&
                                                                                    (e.UserId == userId));
                if (oldRegister == null)
                    return NotFound();

                _context.DailyRegister.Remove(oldRegister);
                await _context.SaveChangesAsync();
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