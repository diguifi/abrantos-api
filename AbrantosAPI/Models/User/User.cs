using System.Collections.Generic;
using AbrantosAPI.Models.Register;
using Microsoft.AspNetCore.Identity;

namespace AbrantosAPI.Models.User
{
    public class User : IdentityUser
    {
        public IList<DailyRegister> DailyRegisters { get; set; }
    }
}