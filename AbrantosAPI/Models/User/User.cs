using System.Collections.Generic;
using System.Text.RegularExpressions;
using AbrantosAPI.Models.Register;
using Microsoft.AspNetCore.Identity;

namespace AbrantosAPI.Models.User
{
    public class User : IdentityUser
    {
        public IList<DailyRegister> DailyRegisters { get; set; }

        public bool IsValidEmail(string email)
        {
            if (!string.IsNullOrEmpty(email))
                return Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            return false;
        }
    }
}