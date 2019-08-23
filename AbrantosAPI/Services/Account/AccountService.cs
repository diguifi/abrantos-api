using System.Linq;
using System.Threading.Tasks;
using AbrantosAPI.Data;
using AbrantosAPI.Models.User;
using AbrantosAPI.Services.Account.Dtos;
using AbrantosAPI.Utils.Pagination;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AbrantosAPI.Services.Account
{
    public class AccountService : IAccountService
    {
        private readonly AbrantosContext _context;
        private readonly UserManager<User> _userManager;

        public AccountService(AbrantosContext context,
                                UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<PagedOutput<AspNetFriends>> GetFriends(int page, string userId)
        {
            return await _context.AspNetFriends.Include(e => e.FriendTo) 
                                                    .Where(e => e.FriendFrom.Id == userId && e.IsConfirmed == true)
                                                    .GetPagedAsync(page);
        }
    }
}