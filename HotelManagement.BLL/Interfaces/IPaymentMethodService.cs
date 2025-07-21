
using HotelManagement.Domain.Entities;

namespace HotelManagement.BLL.Interfaces
{
    public interface IPaymentMethodService
    {
        Task<PaymentMethod?> GetPaymentMethodByIdAsync(int id);
        Task<IEnumerable<PaymentMethod>> GetAllPaymentMethodAsync();
        Task<int?> AddPaymentMethodAsync(PaymentMethod paymentMethod);
        Task<bool> UpdatePaymentMethodAsync(PaymentMethod paymentMethod);
        Task<bool> DeletePaymentMethodAsync(int id);
    }
}
