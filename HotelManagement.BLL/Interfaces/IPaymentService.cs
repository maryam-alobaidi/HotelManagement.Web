using HotelManagement.Domain.Entities;

namespace HotelManagement.BLL.Interfaces
{
    public interface IPaymentService
    {
        Task<Payment?> GetPaymentByIdAsync(int id);
        Task<IEnumerable<Payment>> GetAllPaymentssAsync();
        Task<int?> AddPaymentAsync(Payment payments);
        Task<bool> UpdatePaymentAsync(Payment payments);
        Task<bool> DeletePaymentAsync(int id);

    }
}
