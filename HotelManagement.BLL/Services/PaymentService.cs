using HotelManagement.BLL.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Domain.Enums;
using HotelManagement.Infrastructure.DAL.DTOs;
using HotelManagement.Infrastructure.DAL.Interfaces;

namespace HotelManagement.BLL.Services
{
    public class PaymentService : IPaymentService
    {
        // This class is a placeholder for the actual implementation of the payment service.
        private readonly IPaymentsRepository _paymentsRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        public PaymentService(IPaymentsRepository paymentsRepository ,IBookingRepository bookingRepository,IRoomRepository roomRepository, IInvoiceRepository invoiceRepository)
        {
            _paymentsRepository = paymentsRepository ?? throw new ArgumentNullException(nameof(paymentsRepository));
            _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
            _roomRepository= roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
            _invoiceRepository = invoiceRepository ?? throw new ArgumentNullException(nameof(invoiceRepository));
        }



        public async Task<int?> AddPaymentAsync(Payment payment)
        {
            Invoice? invoice = await _invoiceRepository.GetByIdAsync(payment.InvoiceID);
            if (invoice == null)
            {
                throw new KeyNotFoundException($"Invoice with ID {payment.InvoiceID} not found. Cannot add payment.");
            }

            Booking? booking = await _bookingRepository.GetByIdAsync(invoice.BookingID);
            if (booking == null)
            {
                throw new KeyNotFoundException($"Booking with ID {invoice.BookingID} not found.");
            }

            // You can now reuse your existing `GetTotalPaidForBookingAsync` method to get the total paid.
            decimal totalPaidSoFar = await _paymentsRepository.GetTotalPaidForBookingAsync(booking.BookingID);
            decimal totalBookingPrice = booking.TotalPrice;
            decimal outstandingBalance = totalBookingPrice - totalPaidSoFar;

            if (payment.Amount > outstandingBalance + 0.01M)
            {
                throw new InvalidOperationException(
                    $"Payment amount {payment.Amount:C} exceeds the remaining balance for booking {booking.BookingID}. Outstanding: {outstandingBalance:C}");
            }

            // Core operation: Add the new payment to the repository
            int? newPaymentId = await _paymentsRepository.AddAsync(payment);

            // Now, update the Invoice object using its own methods.
            // First, update the total amount paid on the invoice.
            invoice.AmountPaid = (invoice.AmountPaid ?? 0) + payment.Amount;

            // Then, call the invoice's internal methods to update its state.
            invoice.UpdateBalanceDue();
            invoice.UpdateInvoiceStatus();

            // Finally, save the updated invoice state to the database.
            await _invoiceRepository.UpdateAsync(invoice);

            // Update booking status if fully paid
            decimal updatedTotalPaid = await _paymentsRepository.GetTotalPaidForBookingAsync(booking.BookingID);
            if (updatedTotalPaid >= totalBookingPrice && booking.BookingStatus == BookingStatusEnum.Confirmed)
            {
                booking.BookingStatus = BookingStatusEnum.Completed;
                await _bookingRepository.UpdateAsync(booking);
            }

            return newPaymentId;
        }


        public async Task<bool> DeletePaymentAsync(int id)
        {
            if (id <= 0) throw  new ArgumentException($"Payment ID  must be greater than zero.", nameof(id));

           return await _paymentsRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentssAsync()
        {
           return await _paymentsRepository.GetAllAsync();
        }

        public async Task<Payment?> GetPaymentByIdAsync(int id)
        {
            if(id <= 0) throw new ArgumentException($"Payment ID  must be greater than zero.",nameof(id));

            return await _paymentsRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdatePaymentAsync(Payment payments)
        {
            if (payments.PaymentID <= 0) throw new ArgumentException($"Payment ID  must be greater than zero.", nameof(payments.PaymentID));

          return  await _paymentsRepository.UpdateAsync(payments);
        }


        public async Task<IEnumerable<PaymentDetailsDTO>> GetPaymentDetailsAsync()
        {
          return await _paymentsRepository.GetPaymentDetailsDTOsAsync();
        }

        public async Task<PaymentDetailsDTO> GetPaymentDetailsByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException($"Payment ID  must be greater than zero.", nameof(id));
            return await _paymentsRepository.GetPaymentDetailsByIdAsync(id);
        }
    }
}

