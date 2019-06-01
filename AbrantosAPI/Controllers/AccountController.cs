using System;
using System.Linq;
using System.Threading.Tasks;
using AbrantosAPI.Data;
using AbrantosAPI.Models.User;
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
        private readonly AbrantosContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContext;
        public AccountController(AbrantosContext context,
                                    UserManager<User> userManager,
                                    IHttpContextAccessor httpContext)
        {
            _context = context;
            _userManager = userManager;
            _httpContext = httpContext;
        }
        
        [HttpGet("Friends")]
        public async Task<IActionResult> GetFriends(int page)
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            try
            {
                var friends = await _context.AspNetFriends.Include(e => e.FriendTo) 
                                                        .Where(e => e.FriendFrom.Id == userId && e.IsConfirmed == true)
                                                        .GetPagedAsync(page);

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
                var friend = await _context.AspNetFriends.Include(e => e.FriendTo)
                                                        .Where(e => e.FriendFrom.Id == userId && 
                                                        e.FriendTo.UserName == friendUserName &&
                                                        e.IsConfirmed == true)
                                                        .FirstOrDefaultAsync();

                if (friend == null)
                    return NotFound();

                return StatusCode(200, new
                {
                    friend
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal error:" + e.Message);
            }
            
        }

        [HttpPost("Friends/{friendUserName}")]
        public async Task<IActionResult> AddFriend(string friendUserName)
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            try
            {
                var friendFrom = await _userManager.FindByIdAsync(userId);
                var friendTo = await _userManager.FindByNameAsync(friendUserName);

                if (friendTo == null)
                    return StatusCode(404, "User not found");

                var friendRequestObject = new AspNetFriends(friendFrom, friendTo);

                var friendRequest = _context.AspNetFriends.Add(friendRequestObject);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal error:" + e.Message);
            }
            
        }

        [HttpDelete("Friends/{friendUserName}")]
        public async Task<IActionResult> DeleteFriend(string friendUserName)
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            try
            {
                var sentRequest = await _context.AspNetFriends.Where(e => e.FriendFrom.Id == userId && 
                                                        e.FriendTo.UserName == friendUserName &&
                                                        e.IsConfirmed == true)
                                                        .FirstOrDefaultAsync();

                var receivedRequest = await _context.AspNetFriends.Where(e => e.FriendTo.Id == userId && 
                                                        e.FriendFrom.UserName == friendUserName &&
                                                        e.IsConfirmed == true)
                                                        .FirstOrDefaultAsync();

                if (sentRequest == null || receivedRequest == null)
                    return NotFound();

                _context.AspNetFriends.Remove(sentRequest);
                _context.AspNetFriends.Remove(receivedRequest);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal error:" + e.Message);
            }
            
        }

        [HttpGet("Friends/ReceivedRequests")]
        public async Task<IActionResult> GetReceivedRequests()
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            try
            {
                var requests = await _context.AspNetFriends.Include(e => e.FriendFrom)
                                                            .Where(e => e.FriendTo.Id == userId &&
                                                            e.IsConfirmed == false)
                                                            .ToListAsync();

                return StatusCode(200, new
                {
                    requests
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal error:" + e.Message);
            }
        }

        [HttpPost("Friends/ReceivedRequests/{requestId}")]
        public async Task<IActionResult> AcceptReceivedRequest(int requestId)
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            try
            {
                var pendingRequest = await _context.AspNetFriends.Include(e => e.FriendFrom)
                                                                    .Where(e => e.FriendTo.Id == userId &&
                                                                    e.IsConfirmed == false &&
                                                                    e.Id == requestId)
                                                                    .FirstOrDefaultAsync();
                if (pendingRequest == null)
                    return NotFound();

                pendingRequest.IsConfirmed = true;
                _context.AspNetFriends.Update(pendingRequest);

                var friendFrom = await _userManager.FindByIdAsync(userId);
                var friendTo = await _userManager.FindByIdAsync(pendingRequest.FriendFrom.Id);
                var friendRequestObject = new AspNetFriends(friendFrom, friendTo);
                friendRequestObject.IsConfirmed = true;
                var friendRequest = _context.AspNetFriends.Add(friendRequestObject);

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal error:" + e.Message);
            }
        }

        [HttpDelete("Friends/ReceivedRequests/{requestId}")]
        public async Task<IActionResult> DeleteReceivedRequest(int requestId)
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            try
            {
                var request = await _context.AspNetFriends.Include(e => e.FriendFrom)
                                                        .Where(e => e.FriendTo.Id == userId &&
                                                        e.IsConfirmed == false &&
                                                        e.Id == requestId)
                                                        .FirstOrDefaultAsync();

                if (request == null)
                    return NotFound();

                _context.AspNetFriends.Remove(request);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal error:" + e.Message);
            }
        }

        [HttpGet("Friends/SentRequests")]
        public async Task<IActionResult> GetSentRequests()
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            try
            {
                var requests = await _context.AspNetFriends.Include(e => e.FriendTo)
                                                            .Where(e => e.FriendFrom.Id == userId &&
                                                            e.IsConfirmed == false)
                                                            .ToListAsync();

                return StatusCode(200, new
                {
                    requests
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal error:" + e.Message);
            }
        }

        [HttpDelete("Friends/SentRequests/{requestId}")]
        public async Task<IActionResult> DeleteSentRequest(int requestId)
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            try
            {
                var request = await _context.AspNetFriends.Include(e => e.FriendTo)
                                                        .Where(e => e.FriendFrom.Id == userId &&
                                                        e.IsConfirmed == false &&
                                                        e.Id == requestId)
                                                        .FirstOrDefaultAsync();

                if (request == null)
                    return NotFound();

                _context.AspNetFriends.Remove(request);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal error:" + e.Message);
            }
        }
    }
}