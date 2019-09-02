using System.Collections.Generic;
using System.Threading.Tasks;
using AbrantosAPI.Models.User;
using AbrantosAPI.Services.Account.Dtos;
using AbrantosAPI.Utils.Pagination;

namespace AbrantosAPI.Services.Account
{
    public interface IAccountService
    {
        Task<PagedOutput<AspNetFriends>> GetFriends(int page, string userId);
        Task<FriendDto> GetFriend(string userId, string friendUserName);
        Task AddFriend(string userId, string friendUserName);
        Task DeleteFriend(string userId, string friendUserName);
        Task<List<AspNetFriends>> GetReceivedRequests(string userId);
        Task AcceptReceivedRequest(string userId, int requestId);
        Task DeleteReceivedRequest(string userId, int requestId);
        Task<List<AspNetFriends>> GetSentRequests(string userId);
        Task DeleteSentRequest(string userId, int requestId);
    }
}