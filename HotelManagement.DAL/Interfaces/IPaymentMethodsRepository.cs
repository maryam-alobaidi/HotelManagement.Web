
using HotelManagement.Domain.Entities;

namespace HotelManagement.Infrastructure.DAL.Interfaces
{
    public interface IPaymentMethodsRepository
    {
        Task<int?> AddAsync(PaymentMethod paymentMethod);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateAsync(PaymentMethod paymentMethod);
        Task<IEnumerable<PaymentMethod>> GetAllAsync();
        Task<PaymentMethod?> GetByIdAsync(int id);
    }
}
