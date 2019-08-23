using System.Threading.Tasks;
using AbrantosAPI.Models.User;
using AbrantosAPI.Utils.Pagination;

namespace AbrantosAPI.Services.Account
{
    public interface IAccountService
    {
        Task<PagedOutput<AspNetFriends>> GetFriends(int page, string userId);
    }
}