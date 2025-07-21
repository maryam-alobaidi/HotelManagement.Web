using HotelManagement.BLL.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Infrastructure.DAL.Interfaces;


namespace HotelManagement.BLL.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IPaymentMethodsRepository _paymentMethodsRepository;

        public PaymentMethodService(IPaymentMethodsRepository paymentMethodsRepository)
        {
            _paymentMethodsRepository = paymentMethodsRepository ?? throw new ArgumentNullException(nameof(paymentMethodsRepository));
        }
      
        public async Task<int?> AddPaymentMethodAsync(PaymentMethod paymentMethod)
        {
            if (paymentMethod == null) throw new ArgumentNullException(nameof(paymentMethod), "Payment Method cannot be null.");
            return await _paymentMethodsRepository.AddAsync(paymentMethod);
        }

        public async Task<bool> DeletePaymentMethodAsync(int id)
        {
            if (id <= 0) throw new ArgumentException($"Payment Method ID must be greater than zero.", nameof(id));

           return   await _paymentMethodsRepository.DeleteAsync(id);
        }

        public async Task<PaymentMethod?> GetPaymentMethodByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException($"Payment Method ID  must be greater than zero.", nameof(id));

            return await _paymentMethodsRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<PaymentMethod>> GetAllPaymentMethodAsync()
        {
           return await _paymentMethodsRepository.GetAllAsync();
        }

        public async Task<bool> UpdatePaymentMethodAsync(PaymentMethod paymentMethod)
        {
            if (paymentMethod.MethodID <= 0) throw new ArgumentException($"Payment Method ID must be greater than zero.", nameof(paymentMethod.MethodID));

           return await _paymentMethodsRepository.UpdateAsync(paymentMethod);
        }
    }
}
