using FanQuest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Application.Interfaces.Repositories
{
    public interface IChallengeRepository
    {
        Task<Challenge> CreateAsync(Challenge challenge);
        Task<Challenge?> GetByIdAsync(Guid id);
    }
}
