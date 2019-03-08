using AbrantosAPI.Models.Register;
using Microsoft.EntityFrameworkCore;

namespace AbrantosAPI.Data
{
    public class AbrantosContext : DbContext     
  {         
    public AbrantosContext(DbContextOptions<AbrantosContext> options) : base(options)         
    {
    }
    

    public DbSet<DailyRegister> DailyRegister { get; set; }
  } 
}