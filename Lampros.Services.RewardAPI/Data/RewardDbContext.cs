
using Lampros.Services.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Lampros.Services.RewardAPI.Data
{
    public class RewardDbContext: DbContext
    {
        public RewardDbContext(DbContextOptions<RewardDbContext> options) : base(options)
        {
            
        }

        public DbSet<Reward> Rewards { get; set; }
    }
}
