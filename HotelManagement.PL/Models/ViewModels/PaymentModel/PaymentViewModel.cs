using HotelManagement.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Web.Models.ViewModels.PaymentModel
{
    /// <summary>
    /// ViewModel for displaying payment details. This model is optimized for read-only
    /// views like a details page or a list of payments. It includes navigation properties
    /// to easily access related data in the view.
    /// </summary>
    public class PaymentViewModel
    {
        [Display(Name = "Payment ID")]
        public int PaymentID { get; set; }

        [Display(Name = "Invoice ID")]
        public int InvoiceID { get; set; }

        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Payment Date")]
        public DateTime? PaymentDate { get; set; }

        [Display(Name = "Payment Method")]
        public int PaymentMethodID { get; set; }

        [Display(Name = "Transaction Reference")]
        public string? TransactionReference { get; set; }

        [Display(Name = "Recorded by Employee")]
        public int? RecordedByEmployeeID { get; set; }

        // Navigation properties for displaying related data.
        // These are populated by the controller to allow the view to
        // directly access details like names without extra lookups.
        public Employee? Employee { get; set; }

        public Invoice? Invoice { get; set; }

        public PaymentMethod? PaymentMethod { get; set; }
    }
}
