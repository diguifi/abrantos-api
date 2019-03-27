using System.Linq;
using System.Threading.Tasks;
using AbrantosAPI.Data;
using AbrantosAPI.Models.User;
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
        public async Task<IActionResult> GetFriends()
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            var friends = await _context.AspNetFriends.Include(e => e.FriendTo) 
                                                    .Where(e => e.FriendFrom.Id == userId && e.IsConfirmed == true)
                                                    .ToListAsync();

            return StatusCode(200, new
            {
                friends
            });
        }

        [HttpGet("Friends/{friendUserName}")]
        public async Task<IActionResult> GetFriend(string friendUserName)
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            var friend = await _context.AspNetFriends.Include(e => e.FriendTo) 
                                                    .Where(e => e.FriendFrom.Id == userId && 
                                                    e.FriendTo.UserName == friendUserName &&
                                                    e.IsConfirmed == true)
                                                    .FirstOrDefaultAsync();

            return StatusCode(200, new
            {
                friend
            });
        }

        [HttpGet("Friends/Requests")]
        public async Task<IActionResult> GetRequests()
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            var requests = await _context.AspNetFriends.Include(e => e.FriendFrom)
                                                        .Where(e => e.FriendTo.Id == userId &&
                                                        e.IsConfirmed == false)
                                                        .ToListAsync();

            return StatusCode(200, new
            {
                requests
            });
        }

        [HttpPost("Friends/{friendUserName}")]
        public async Task<IActionResult> AddFriend(string friendUserName)
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;

            var friendFrom = await _userManager.FindByIdAsync(userId);
            var friendTo = await _userManager.FindByNameAsync(friendUserName);

            var friendRequestObject = new AspNetFriends(friendFrom, friendTo);

            var friendRequest = _context.AspNetFriends.Add(friendRequestObject);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("Friends/Requests/{requestId}")]
        public async Task<IActionResult> AcceptRequest(int requestId)
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
            
            var pendingRequest = await _context.AspNetFriends.Include(e => e.FriendFrom)
                                                            .Where(e => e.FriendTo.Id == userId &&
                                                                    e.IsConfirmed == false &&
                                                                    e.Id == requestId)
                                                                    .FirstOrDefaultAsync();
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
    }
}