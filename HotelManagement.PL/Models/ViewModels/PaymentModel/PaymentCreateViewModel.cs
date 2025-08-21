using HotelManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Web.Models.ViewModels.PaymentModel
{
    public class PaymentCreateViewModel
    {
        // ... (Other properties remain unchanged)

        /// <summary>
        /// The unique identifier for the payment.
        /// It is nullable because it will be null for new payments (Create action).
        /// </summary>
        [Display(Name = "Payment ID")]
        public int? PaymentID { get; set; }


        [Display(Name = "Invoice ID")]
        public int? InvoiceID { get; set; }

        [Display(Name = "Amount")]
        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Amount must be a positive number.")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Display(Name = "Payment Date")]
        [DataType(DataType.DateTime)]
        public DateTime? PaymentDate { get; set; }=DateTime.Now;

        [Display(Name = "Payment Method")]
        [Required(ErrorMessage = "Payment Method is required.")]
        public int PaymentMethodID { get; set; }

        [Display(Name = "Transaction Reference")]
        public string? TransactionReference { get; set; }

        [Display(Name = "Recorded by Employee")]
        public int? RecordedByEmployeeID { get; set; }

        [Display(Name = "Booking")]
        [Required(ErrorMessage = "A booking is required.")]
        public int? BookingID { get; set; }

        public SelectList? UnpaidBookings { get; set; }
        public IEnumerable<SelectListItem>? BookingList { get; set; }
        public IEnumerable<SelectListItem>? InvoiceList { get; set; }
        public IEnumerable<SelectListItem>? PaymentMethodList { get; set; }
        public IEnumerable<SelectListItem>? EmployeeList { get; set; }
    }
}
