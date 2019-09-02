using System;
using System.Collections.Generic;
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

        public async Task<FriendDto> GetFriend(string userId, string friendUserName)
        {
            var friend = await _context.AspNetFriends.Include(e => e.FriendTo)
                                                .ThenInclude(d => d.DailyRegisters)
                                                .Where(e => e.FriendFrom.Id == userId && 
                                                e.FriendTo.UserName == friendUserName &&
                                                e.IsConfirmed == true)
                                                .FirstOrDefaultAsync();

            if (friend == null)
                throw new Exception("Usuário não encontrado");

            return new FriendDto()
            {
                Email = friend.FriendTo.Email,
                UserName = friend.FriendTo.UserName,
                DailyRegisters = friend.FriendTo.DailyRegisters
            };
        }

        public async Task AddFriend(string userId, string friendUserName)
        {
            var friendFrom = await _userManager.FindByIdAsync(userId);
            var friendTo = await _userManager.FindByNameAsync(friendUserName);

            if (friendTo == null)
                throw new Exception("Usuário não encontrado");

            var friendRequestObject = new AspNetFriends(friendFrom, friendTo);

            var friendRequest = _context.AspNetFriends.Add(friendRequestObject);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFriend(string userId, string friendUserName)
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
                throw new Exception("Usuário não encontrado");

            _context.AspNetFriends.Remove(sentRequest);
            _context.AspNetFriends.Remove(receivedRequest);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AspNetFriends>> GetReceivedRequests(string userId)
        {
            return await _context.AspNetFriends.Include(e => e.FriendFrom)
                                                .Where(e => e.FriendTo.Id == userId &&
                                                e.IsConfirmed == false)
                                                .ToListAsync();
        }

        public async Task AcceptReceivedRequest(string userId, int requestId)
        {
            var pendingRequest = await _context.AspNetFriends.Include(e => e.FriendFrom)
                                                                .Where(e => e.FriendTo.Id == userId &&
                                                                e.IsConfirmed == false &&
                                                                e.Id == requestId)
                                                                .FirstOrDefaultAsync();
            if (pendingRequest == null)
                throw new Exception("Usuário não encontrado");

            pendingRequest.IsConfirmed = true;
            _context.AspNetFriends.Update(pendingRequest);

            var friendFrom = await _userManager.FindByIdAsync(userId);
            var friendTo = await _userManager.FindByIdAsync(pendingRequest.FriendFrom.Id);
            var friendRequestObject = new AspNetFriends(friendFrom, friendTo);
            friendRequestObject.IsConfirmed = true;
            var friendRequest = _context.AspNetFriends.Add(friendRequestObject);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteReceivedRequest(string userId, int requestId)
        {
            var request = await _context.AspNetFriends.Include(e => e.FriendFrom)
                                                    .Where(e => e.FriendTo.Id == userId &&
                                                    e.IsConfirmed == false &&
                                                    e.Id == requestId)
                                                    .FirstOrDefaultAsync();

            if (request == null)
                throw new Exception("Usuário não encontrado");

            _context.AspNetFriends.Remove(request);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AspNetFriends>> GetSentRequests(string userId)
        {
            return await _context.AspNetFriends.Include(e => e.FriendTo)
                                            .Where(e => e.FriendFrom.Id == userId &&
                                            e.IsConfirmed == false)
                                            .ToListAsync();
        }

        public async Task DeleteSentRequest(string userId, int requestId)
        {
            var request = await _context.AspNetFriends.Include(e => e.FriendTo)
                                                    .Where(e => e.FriendFrom.Id == userId &&
                                                    e.IsConfirmed == false &&
                                                    e.Id == requestId)
                                                    .FirstOrDefaultAsync();

            if (request == null)
                throw new Exception("Usuário não encontrado");

            _context.AspNetFriends.Remove(request);
            await _context.SaveChangesAsync();
        }
    }
}