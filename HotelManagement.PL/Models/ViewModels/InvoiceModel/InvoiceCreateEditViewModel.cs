using HotelManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Web.Models.ViewModels.InvoiceModel
{
    public class InvoiceCreateEditViewModel
    {
     

        public List<SelectListItem> Bookings { get; set; } = new List<SelectListItem>();

        // هذه الخاصية ستحمل قيمة رقم الحجز الذي اختاره المستخدم
        [Display(Name = "Booking")]
        [Required(ErrorMessage = "Please select a booking.")]
        public int BookingID { get; set; }

        // الخصائص الأخرى الخاصة بالفاتورة
        public int InvoiceID { get; set; }

        [Display(Name = "Customer")]
        public string? CustomerName { get; set; }
        public int? CustomerID { get; set; }

        [Display(Name = "Employee")]
        public string? EmployeeName { get; set; }
        public int? GeneratedByEmployeeID { get; set; }

        [Display(Name = "Invoice Date")]
        [Required(ErrorMessage = "Invoice date is required.")]
        [DataType(DataType.Date)]
        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        [Display(Name = "Total Amount")]
        [Required(ErrorMessage = "Total amount is required.")]
        [Range(0, 999999.99, ErrorMessage = "Total amount must be a positive value.")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

      
        [Display(Name = "Amount Paid")]
        [Required(ErrorMessage = "Amount paid is required.")]
        [Range(0, 999999.99, ErrorMessage = "Amount paid must be a positive value.")]
        [DataType(DataType.Currency)]
        public decimal AmountPaid { get; set; }

        // This is a computed property that calculates the balance due.
        // It does not need to be saved to the database.
        [Display(Name = "Balance Due")]
        [DataType(DataType.Currency)]
        public decimal BalanceDue => TotalAmount - AmountPaid;

        [Display(Name = "Invoice Status")]
        public string InvoiceStatus { get; set; } = "UnPaid";

    

    }
}
