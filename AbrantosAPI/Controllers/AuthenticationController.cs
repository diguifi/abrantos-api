using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using AbrantosAPI.Authentication;
using AbrantosAPI.Data;
using AbrantosAPI.Models.User;
using AbrantosAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AbrantosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AbrantosContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;
        public AuthenticationController(AbrantosContext context,
                                        UserManager<User> userManager,
                                        SignInManager<User> signInManager,
                                        IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
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

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] CreateUserViewModel newUser)
        {
            if (newUser.Password != newUser.PasswordConfirmation)
                return StatusCode(400, "The passwords must match");

            if (!new User().IsValidEmail(newUser.Email))
                return StatusCode(400, "Invalid email format");

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
                        checkUserCreation.Errors
                    });
                }

                var tokenConfirmEmail = await _userManager.GenerateEmailConfirmationTokenAsync(mappedUser);
                var emailBuilder = new EmailBuilder("", "",
                                            mappedUser.Id, 
                                            tokenConfirmEmail);

                await _emailSender.SendEmailAsync(mappedUser.Email,
                                                    EmailBuilder.SubjectConfirmEmail,
                                                    emailBuilder.GetEmailConfirmationMessage());

                return StatusCode(200, "Please confirm your email.");
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

        [HttpPost("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string userId)
        {
            token = token.Replace(" ", "+");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return StatusCode(200, "Email confirmed!");
            }

            return StatusCode(400, new { result.Errors });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<object> Login(
            UserLoginDto userDto,
            [FromServices]SigningConfigurations signingConfigurations,
            [FromServices]TokenConfigurations tokenConfigurations)
        {
            bool validCredentials = false;
            var userInDB = await _userManager.FindByNameAsync(userDto.UserName);
            if (userInDB == null)
                return StatusCode(404, "User not found.");

            if (!userInDB.EmailConfirmed)
                return StatusCode(400, "Please confirm your email first.");

            var loginResult = await _signInManager
                .CheckPasswordSignInAsync(userInDB, userDto.Password, false);

            if (!loginResult.Succeeded)
            {
                return StatusCode(404, "Invalid credentials.");
            }
            else 
            {
                validCredentials = true;
            }

            
            if (validCredentials)
            {
                ClaimsIdentity declarations = new ClaimsIdentity(
                    new GenericIdentity(userInDB.UserName, "Login"),
                    new[] {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                        new Claim(JwtRegisteredClaimNames.UniqueName, userInDB.UserName),
                        new Claim("userid", userInDB.Id, ClaimValueTypes.String),
                        new Claim("username", userInDB.UserName, ClaimValueTypes.String),
                    }
                );

                DateTime creationDate = DateTime.Now;
                DateTime expirationDate = creationDate +
                    TimeSpan.FromSeconds(tokenConfigurations.Seconds);

                var handler = new JwtSecurityTokenHandler();
                var securityToken = handler.CreateToken(new SecurityTokenDescriptor
                {
                    Issuer = tokenConfigurations.Issuer,
                    Audience = tokenConfigurations.Audience,
                    SigningCredentials = signingConfigurations.SigningCredentials,
                    Subject = declarations,
                    NotBefore = creationDate,
                    Expires = expirationDate
                });
                var token = handler.WriteToken(securityToken);

                return new
                {
                    authenticated = true,
                    created = creationDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    expiration = expirationDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    accessToken = token,
                    message = "OK"
                };
            }
            else
            {
                return StatusCode(404, "Invalid credentials.");
            }
        }
    }
}