
using Lampros.Services.RewardAPI.Message;

namespace Lampros.Services.RewardAPI.Services
{
    public interface IRewardService
    {
        Task UpdateRewards(RewardsMessage rewardsMessage);
      
    }
}
