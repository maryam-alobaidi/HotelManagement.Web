using HotelManagement.Domain.Enums;

namespace HotelManagement.Web.Models.ViewModels.PaymentModel
{
    public class PaymentDetailsViewModel
    {
        public int PaymentID { get; set; }
        public int InvoiceID { get; set; }
        public decimal Amount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int PaymentMethodID { get; set; }
        public string? TransactionReference { get; set; }
        public int? RecordedByEmployeeID { get; set; }
        public string Username { get; set; }
        public InvoiceStatusEnum InvoiceStatus { get; set; }
        public string PaymentMethodName { get; set; }
    }
}

