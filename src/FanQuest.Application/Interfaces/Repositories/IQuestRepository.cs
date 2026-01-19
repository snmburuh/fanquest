using FanQuest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Application.Interfaces.Repositories
{
    public interface IQuestRepository
    {
        Task<Quest?> GetByIdAsync(Guid id);
        Task<IEnumerable<Quest>> GetActiveQuestsAsync(string? city = null);
        Task<Quest> CreateAsync(Quest quest);
        Task UpdateAsync(Quest quest);
    }
}
