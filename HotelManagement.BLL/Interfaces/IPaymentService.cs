using HotelManagement.Domain.Entities;
using HotelManagement.Infrastructure.DAL.DTOs;

namespace HotelManagement.BLL.Interfaces
{
    public interface IPaymentService
    {
        Task<Payment?> GetPaymentByIdAsync(int id);
        Task<IEnumerable<Payment>> GetAllPaymentssAsync();
        Task<int?> AddPaymentAsync(Payment payments);
        Task<bool> UpdatePaymentAsync(Payment payments);
        Task<bool> DeletePaymentAsync(int id);
        Task<IEnumerable<PaymentDetailsDTO>> GetPaymentDetailsAsync();
        Task<PaymentDetailsDTO?> GetPaymentDetailsByIdAsync(int id);

    }
}
