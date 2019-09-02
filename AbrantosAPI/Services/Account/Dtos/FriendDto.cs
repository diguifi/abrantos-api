using System.Collections.Generic;
using AbrantosAPI.Models.Register;

namespace AbrantosAPI.Services.Account.Dtos
{
    public class FriendDto
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public IList<DailyRegister> DailyRegisters { get; set; }
    }
}