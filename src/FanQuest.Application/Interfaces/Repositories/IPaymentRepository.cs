using FanQuest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Application.Interfaces.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(Guid id);
        Task<Payment> CreateAsync(Payment payment);
        Task UpdateAsync(Payment payment);
    }
}
