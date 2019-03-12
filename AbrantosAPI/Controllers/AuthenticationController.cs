using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AbrantosAPI.Data;
using AbrantosAPI.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AbrantosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AbrantosContext _context;
        private readonly UserManager<User> _userManager;
        public AuthenticationController(AbrantosContext context,
                                        UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // just a test
        [HttpGet]
        public async Task<ActionResult> GetAdmins()
        {
            try 
            {
                var admins = await _userManager.GetUsersInRoleAsync("Admin");

                return StatusCode(200, new
                {
                    admins
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
    }
}