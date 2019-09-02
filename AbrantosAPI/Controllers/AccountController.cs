using System;
using System.Linq;
using System.Threading.Tasks;
using AbrantosAPI.Data;
using AbrantosAPI.Models.User;
using AbrantosAPI.Services.Account;
using AbrantosAPI.Utils.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AbrantosAPI.Controllers
{
    [Authorize("Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IHttpContextAccessor _httpContext;
        public AccountController(IAccountService accountService,
                                    IHttpContextAccessor httpContext)
        {
            _accountService = accountService;
            _httpContext = httpContext;
        }
        
        [HttpGet("Friends")]
        public async Task<IActionResult> GetFriends(int page)
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            try
            {
                var friends = await _accountService.GetFriends(page, userId);

                return StatusCode(200, new
                {
                    friends
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal error:" + e.Message);
            }
            
        }

        [HttpGet("Friends/{friendUserName}")]
        public async Task<IActionResult> GetFriend(string friendUserName)
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            try
            {
                var friend = await _accountService.GetFriend(userId, friendUserName);

                if (friend == null)
                    return NotFound();

                return StatusCode(200, new
                {
                    friend
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            
        }

        [HttpPost("Friends/{friendUserName}")]
        public async Task<IActionResult> AddFriend(string friendUserName)
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            try
            {
                await _accountService.AddFriend(userId, friendUserName);

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            
        }

        [HttpDelete("Friends/{friendUserName}")]
        public async Task<IActionResult> DeleteFriend(string friendUserName)
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            try
            {
                await _accountService.DeleteFriend(userId, friendUserName);

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            
        }

        [HttpGet("Friends/ReceivedRequests")]
        public async Task<IActionResult> GetReceivedRequests()
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            try
            {
                var requests = await _accountService.GetReceivedRequests(userId);

                return StatusCode(200, new
                {
                    requests
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("Friends/ReceivedRequests/{requestId}")]
        public async Task<IActionResult> AcceptReceivedRequest(int requestId)
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            try
            {
                await _accountService.AcceptReceivedRequest(userId, requestId);

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("Friends/ReceivedRequests/{requestId}")]
        public async Task<IActionResult> DeleteReceivedRequest(int requestId)
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            try
            {
                await _accountService.DeleteReceivedRequest(userId, requestId);

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("Friends/SentRequests")]
        public async Task<IActionResult> GetSentRequests()
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            try
            {
                var requests = await _accountService.GetSentRequests(userId);

                return StatusCode(200, new
                {
                    requests
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("Friends/SentRequests/{requestId}")]
        public async Task<IActionResult> DeleteSentRequest(int requestId)
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            try
            {
                await _accountService.DeleteSentRequest(userId, requestId);

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}