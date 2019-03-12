using AbrantosAPI.Models.Register;
using AbrantosAPI.Models.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AbrantosAPI.Data
{
    public class AbrantosContext : IdentityDbContext<User, Role, string>
  {         
    public AbrantosContext(DbContextOptions<AbrantosContext> options) : base(options)         
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
    

    public DbSet<DailyRegister> DailyRegister { get; set; }
  } 
}