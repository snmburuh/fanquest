using FanQuest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByPhoneNumberAsync(string phoneNumber);
        Task<User> CreateAsync(User user);
        Task UpdateAsync(User user);
    }
}
