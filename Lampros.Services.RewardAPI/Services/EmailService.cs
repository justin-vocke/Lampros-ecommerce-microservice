

using Lampros.Services.RewardAPI.Data;
using Lampros.Services.RewardAPI.Message;
using Lampros.Services.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace Lampros.Services.RewardAPI.Services
{
    public class RewardService : IRewardService
    {
        private DbContextOptions<RewardDbContext> _dbOptions;

        public RewardService(DbContextOptions<RewardDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

                

        public async Task UpdateRewards(RewardsMessage rewardsMessage)
        {
            try
            {
                Reward reward = new()
                {
                    OrderId = rewardsMessage.OrderId,
                    UserId = rewardsMessage.UserId,
                    RewardActivity = rewardsMessage.RewardActivity,
                    RewardDate = DateTime.Now,
                };
                await using var _db = new RewardDbContext(_dbOptions);
                _db.Rewards.Add(reward);
                await _db.SaveChangesAsync();
                
            }
            catch (Exception ex)
            {

            }
        }
    }
}
