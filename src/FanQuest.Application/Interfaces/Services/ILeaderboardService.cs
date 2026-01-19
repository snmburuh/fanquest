using FanQuest.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Application.Interfaces.Services
{
    public interface ILeaderboardService
    {
        Task<IEnumerable<LeaderboardEntryDto>> GetLeaderboardAsync(Guid questId, int limit = 50);
        Task UpdateScoreAsync(Guid questId, Guid userId, int score);
        Task<int?> GetUserRankAsync(Guid questId, Guid userId);
    }
}
