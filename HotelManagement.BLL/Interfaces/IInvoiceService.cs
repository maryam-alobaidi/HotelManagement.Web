

using HotelManagement.Domain.Entities;

namespace HotelManagement.BLL.Interfaces
{
    public interface IInvoiceService
    {

        Task<int?> AddInvoiceAsync(Invoice invoice);
        Task <bool> UpdateInvoiceAsync(Invoice invoice);
        Task<bool> DeleteInvoiceAsync(int invoiceId);
        Task<Invoice?> GetInvoiceByIdAsync(int invoiceId);
        Task<IEnumerable<Invoice>> GetAllInvoicesAsync();
        Task<IEnumerable<Invoice>> GetInvoicesByCustomerIdAsync(int customerId);
        Task<decimal?> CalculateInvoiceTotalAsync(int invoiceId);
        Task<bool> MarkInvoiceAsPaidAsync(int invoiceId, decimal amountPaid);


        ///// <summary>
        ///// Generates an invoice for a given reservation.
        ///// </summary>
        ///// <param name="reservationId">The ID of the reservation.</param>
        ///// <returns>A string representing the generated invoice.</returns>
        //string GenerateInvoice(int reservationId);
       

        ///// <summary>
        ///// Sends the invoice to the customer via email.
        ///// </summary>
        ///// <param name="reservationId">The ID of the reservation.</param>
        //void SendInvoiceByEmail(int reservationId);
    }
}
