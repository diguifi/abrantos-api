using Microsoft.AspNetCore.Identity;

namespace AbrantosAPI.Models.User
{
    public class Role : IdentityRole
    {
        public Role(string roleName) : base(roleName)
        {
        }
        protected Role()
        {
        }
    }
}