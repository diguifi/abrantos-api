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

        [HttpGet("admins")]
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

        [HttpGet("users")]
        public async Task<ActionResult> GetUsers()
        {
            try 
            {
                var users = await _userManager.GetUsersInRoleAsync("User");

                return StatusCode(200, new
                {
                    users
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

        [HttpPost("register")]
        public async Task<ActionResult> Post([FromBody] CreateUserViewModel newUser)
        {
            if (newUser.Password != newUser.PasswordConfirmation)
                return StatusCode(400, "The passwords must match");

            var mappedUser = new User() {
                UserName = newUser.UserName,
                Email = newUser.Email
            };

            try
            {
                IdentityResult checkUserCreation = await _userManager.CreateAsync(mappedUser, newUser.Password);
                if (checkUserCreation.Succeeded)
                {
                    _userManager.AddToRoleAsync(mappedUser, Roles.ROLE_USER).Wait();
                }
                else
                {
                    return StatusCode(400, new
                    {
                        mappedUser.UserName
                    });
                }

                return StatusCode(200, new
                {
                    newUser
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