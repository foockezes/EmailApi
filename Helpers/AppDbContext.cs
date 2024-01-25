using MailProject.Models;
using Microsoft.EntityFrameworkCore;

namespace MailProject.Helpers
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<CopyFor> CopyFor { get; set; }
        public DbSet<MessageFile> Files { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {

        }
    }
}
