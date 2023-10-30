using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DemoBlazorServerOnlinePostOffice.Data.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Box> Boxes { get; set; } = default!;
        public DbSet<FileSent> FileSents { get; set; } = default!;
        public DbSet<UserAccount> UserAccounts { get; set; } = default!;
        public DbSet<UserRole> UserRoles { get; set; } = default!;

    }
}
