using HotelManagement.BLL.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Infrastructure.DAL.Interfaces;
using HotelManagement.Domain.Enums;

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
            // 1. Basic validation for Payment properties (invariants)
            if (payment.Amount <= 0)
            {
                throw new ArgumentException("Payment amount must be greater than zero.", nameof(payment.Amount));
            }
            if (payment.PaymentMethodID <= 0)
            {
                throw new ArgumentException("Payment method ID must be greater than zero.", nameof(payment.PaymentMethodID));
            }
            // Check if RecordedByEmployeeID has a value and if that value is invalid
            if (payment.RecordedByEmployeeID.HasValue && payment.RecordedByEmployeeID.Value <= 0)
            {
                throw new ArgumentException("Recorded by Employee ID must be greater than zero if provided.", nameof(payment.RecordedByEmployeeID));
            }

            // Set payment date to UTC Now if not provided
            if (payment.PaymentDate == default)
            {
                payment.PaymentDate = DateTime.UtcNow;
            }

            if (payment.InvoiceID <= 0)
            {
                throw new ArgumentException("Payment must be associated with a valid Invoice ID.", nameof(payment.InvoiceID));
            }

            // Get invice and validate its existence
            Invoice? invoice = await _invoiceRepository.GetByIdAsync(payment.InvoiceID);
            if (invoice == null)
            {
                throw new KeyNotFoundException($"Invoice with ID {payment.InvoiceID} not found. Cannot add payment.");
            }

            // 2. Validate linked entities and their existence
            if (payment.invoice.BookingID <= 0)
            {
                throw new ArgumentException("Payment must be associated with a valid Booking ID.", nameof(payment.invoice.BookingID));
            }

            Booking? booking = await _bookingRepository.GetByIdAsync(invoice.BookingID);
            if (booking == null)
            {
                throw new KeyNotFoundException($"Booking with ID {invoice.BookingID} not found. Cannot add payment.");
            }

            Room? room = await _roomRepository.GetByIdAsync(booking.RoomID);
            if (room == null)
            {
                // This shouldn't typically happen if booking creation is robust, but good to check.
                throw new KeyNotFoundException($"Room with ID {booking.RoomID} not found for booking {booking.BookingID}.");
            }

            // 3. Advanced business rules based on booking status and financial state
            if (booking.BookingStatus == BookingStatusEnum.Cancelled)
            {
                throw new InvalidOperationException($"Cannot add payment for a cancelled booking (ID: {booking.BookingID}).");
            }
            if (booking.BookingStatus == BookingStatusEnum.Completed)
            {
                throw new InvalidOperationException($"Cannot add payment for a completed booking (ID: {booking.BookingID}).");
            }

            // Recalculate Booking's TotalPrice to ensure accuracy with current room price
            decimal currentBookingTotalPrice = booking.CalculateTotalPrice(room.PricePerNight.Value);

            // Get total paid so far and calculate outstanding balance
            decimal totalPaidSoFar = await _paymentsRepository.GetTotalPaidForBookingAsync(booking.BookingID);
            decimal outstandingBalance = currentBookingTotalPrice - totalPaidSoFar;

            // Prevent overpayment
            if (payment.Amount > outstandingBalance + 0.01M) // 0.01M is a small tolerance for decimal comparisons
            {
                throw new InvalidOperationException(
                    $"Payment amount {payment.Amount:C} exceeds the remaining balance for booking {booking.BookingID}. Outstanding: {outstandingBalance:C}");
            }

            // 4. Core Operation: Add the payment
            int? newPaymentId = await _paymentsRepository.AddAsync(payment);

            // 5. Update booking status based on total payments
            decimal updatedTotalPaid = await _paymentsRepository.GetTotalPaidForBookingAsync(booking.BookingID);

            // If the booking is now fully paid and was previously confirmed, mark it as completed
            if (updatedTotalPaid >= currentBookingTotalPrice && booking.BookingStatus == BookingStatusEnum.Confirmed)
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
    }
}
