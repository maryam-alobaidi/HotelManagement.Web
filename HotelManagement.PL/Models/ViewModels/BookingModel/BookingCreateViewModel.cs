using HotelManagement.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Web.Models.ViewModels.BookingModel
{
    public class BookingCreateViewModel
    {

        [Display(Name = "Room")] // Display name for dropdown
        [Required(ErrorMessage = "Room is required.")]
        public int RoomID { get; set; }
        // You'd also need a SelectList for Room dropdowns:
        public IEnumerable<SelectListItem>? Rooms { get; set; }

        [Display(Name = "Customer")] // Display name for dropdown
        [Required(ErrorMessage = "Customer is required.")]
        public int CustomerID { get; set; }
        // You'd also need a SelectList for Customer dropdowns:
        public IEnumerable<SelectListItem>? Customers { get; set; }

        [Display(Name = "Check-in Date")]
        [Required(ErrorMessage = "Check-in date is required.")]
        [DataType(DataType.Date)]
        // Add a future date validation if needed
        public DateTime CheckInDate { get; set; }

        [Display(Name = "Check-out Date")]
        [Required(ErrorMessage = "Check-out date is required.")]
        [DataType(DataType.Date)]
        // Add validation to ensure CheckOutDate > CheckInDate
        public DateTime CheckOutDate { get; set; }

        // BookingDate might be set in the controller/service automatically, not taken from user input
        // Or if it's user input, it might default to today's date

        [Display(Name = "Number of Adults")]
        [Required(ErrorMessage = "Number of adults is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Number of adults must be at least 1.")]
        public int NumAdults { get; set; }

        [Display(Name = "Number of Children")]
        [Range(0, int.MaxValue, ErrorMessage = "Number of children cannot be negative.")]
        public int? NumChildren { get; set; }

        // TotalPrice might be calculated by the system, not directly entered by user
        // If it is, then validation is fine.
        [Display(Name = "Total Price")]
        [Required(ErrorMessage = "Total price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total price must be greater than zero.")]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }

        // BookingStatus for creation might default to 'Pending' and not be selected by user
        // If user selects it (e.g., Employee creating confirmed booking), then this is fine.
        [Display(Name = "Booking Status")]
        
        [EnumDataType(typeof(BookingStatusEnum), ErrorMessage = "Invalid booking status.")]
        public BookingStatusEnum BookingStatus { get; set; }

        [Display(Name = "Booked By Employee")] // Display name for dropdown
        public int? BookedByEmployeeID { get; set; }
        // You'd also need a SelectList for Employee dropdowns:
        public IEnumerable<SelectListItem>? Employees { get; set; }

       
    }
}
