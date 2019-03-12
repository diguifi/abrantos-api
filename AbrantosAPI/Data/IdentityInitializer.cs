using Microsoft.AspNetCore.Identity;
using AbrantosAPI.Models.User;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AbrantosAPI.Data
{
    public class IdentityInitializer
    {
        private readonly AbrantosContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;


        public IdentityInitializer(
            AbrantosContext context,
            UserManager<User> userManager,
            RoleManager<Role> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Initialize()
        {
            _context.Database.Migrate();

            if (!_roleManager.RoleExistsAsync(Roles.ROLE_ADMIN).Result)
            {
                var resultado = _roleManager.CreateAsync(
                    new Role(Roles.ROLE_ADMIN)).Result;
                if (!resultado.Succeeded)
                {
                    throw new Exception(
                        $"Error creating role {Roles.ROLE_ADMIN}.");
                }
            }

            if (!_roleManager.RoleExistsAsync(Roles.ROLE_USER).Result)
            {
                var resultado = _roleManager.CreateAsync(
                    new Role(Roles.ROLE_USER)).Result;
                if (!resultado.Succeeded)
                {
                    throw new Exception(
                        $"Error creating role {Roles.ROLE_USER}.");
                }
            }


            await CreateUserAsync(
                new User()
                {
                    UserName = "admin",
                    Email = "admin@admin.com.br",
                    EmailConfirmed = true,
                }, "123@Qwe", new List<string> { Roles.ROLE_ADMIN });
        }

        private async Task CreateUserAsync(
            User user,
            string password,
            List<string> initialRoles)
        {
            if (!_userManager.Users.IgnoreQueryFilters().Any(x => x.UserName == user.UserName))
            {
                var result = await _userManager
                    .CreateAsync(user, password);

                if (result.Succeeded && initialRoles.Count > 0)
                {
                    await _userManager.AddToRolesAsync(user, initialRoles);
                }
            }
        }
    }
}