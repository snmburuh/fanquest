using FanQuest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Application.Interfaces.Repositories
{
    public interface IParticipationRepository
    {
        Task<Participation?> GetByIdAsync(Guid id);
        Task<Participation?> GetByUserAndQuestAsync(Guid userId, Guid questId);
        Task<bool> HasCompletedChallengeAsync(Guid userId, Guid challengeId);
        Task<Participation> CreateAsync(Participation participation);
        Task UpdateAsync(Participation participation);
    }
}
