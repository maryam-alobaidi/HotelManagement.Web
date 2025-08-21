using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Web.Models.ViewModels.PaymentModel
{
    public class PaymentEditViewModel
    {
        // Payment ID is essential for editing, but should be hidden in the view.
        [Required]
        public int PaymentID { get; set; }

        // The Invoice ID and Booking ID should be read-only in the edit view
        // to prevent changes to the fundamental relationship.
        [Required]
        public int? InvoiceID { get; set; }

        [Required]
        public int? BookingID { get; set; }

        // These properties are editable
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be a positive value.")]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; }= DateTime.Now;

        // The PaymentMethodID needs to be required as it's a dropdown.
        [Required(ErrorMessage = "A payment method is required.")]
        public int PaymentMethodID { get; set; }

        // This is a string and can be optional
        public string? TransactionReference { get; set; }

        // This is typically a read-only field
        public int? RecordedByEmployeeID { get; set; }


        // Add this property to hold the dropdown list items
        public List<SelectListItem> PaymentMethodsList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> EmployeeList { get; set; } = new List<SelectListItem>();
    }
}

