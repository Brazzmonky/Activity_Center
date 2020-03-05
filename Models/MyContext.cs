using Microsoft.EntityFrameworkCore;

namespace Activity_center.Models
{
    public class MyContext : DbContext
    {
        public MyContext (DbContextOptions options) : base(options) {}
        public DbSet<User> Users {get;set;}
        public DbSet<Shindig> Shindigs {get;set;}
        public DbSet<Response> Responses{get;set;}

    }
}        
