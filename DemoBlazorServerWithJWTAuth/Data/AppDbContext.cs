using DemoBlazorServerWithJWTAuth.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoBlazorServerWithJWTAuth.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<ApplicationUser> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
