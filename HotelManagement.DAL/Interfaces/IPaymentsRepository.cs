using HotelManagement.Domain.Entities;

namespace HotelManagement.Infrastructure.DAL.Interfaces
{
    public interface IPaymentsRepository
    {
        Task<int?> AddAsync(Payment payment);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateAsync(Payment payment);
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<Payment?> GetByIdAsync(int id);
        Task<decimal> GetTotalPaidForBookingAsync(int bookingId);
    }
}
