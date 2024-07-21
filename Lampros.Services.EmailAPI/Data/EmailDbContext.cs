


using Lampros.Services.EmailAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Lampros.Services.EmailAPI.Data
{
    public class EmailDbContext: DbContext
    {
        public EmailDbContext(DbContextOptions<EmailDbContext> options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
        }

        public DbSet<EmailLogger> EmailLoggers { get; set; }
    }
}
