using FanQuest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Application.Interfaces.Repositories
{
    public interface IRewardRepository
    {
        Task<IEnumerable<Reward>> GetRewardsByQuestIdAsync(Guid questId);
        Task<Reward?> GetByIdAsync(Guid id);
        Task<Reward> CreateAsync(Reward reward);
    }
}
