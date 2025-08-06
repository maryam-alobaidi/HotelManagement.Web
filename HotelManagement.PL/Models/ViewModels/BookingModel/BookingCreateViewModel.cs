using HotelManagement.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Web.Models.ViewModels.BookingModel
{
    public class BookingCreateViewModel
    {

        [Display(Name = "Room")]
        [Required(ErrorMessage = "Room is required.")]
        public int RoomID { get; set; }
     

        [Display(Name = "Customer")]
        [Required(ErrorMessage = "Customer is required.")]
        public int CustomerID { get; set; }
  

        [Display(Name = "Check-in Date")]
        [Required(ErrorMessage = "Check-in date is required.")]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [Display(Name = "Check-out Date")]
        [Required(ErrorMessage = "Check-out date is required.")]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }

        [Display(Name = "Number of Adults")]
        [Required(ErrorMessage = "Number of adults is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Number of adults must be at least 1.")]
        public int NumAdults { get; set; }

        [Display(Name = "Number of Children")]
        [Range(0, int.MaxValue, ErrorMessage = "Number of children cannot be negative.")]
        public int? NumChildren { get; set; } = 0;

        [Display(Name = "Total Price")]
      
      
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }


        [Display(Name = "Booked By Employee")]
        public int? BookedByEmployeeID { get; set; }
        public IEnumerable<SelectListItem>? AvailableRooms { get; set; }
        public IEnumerable<SelectListItem>? AvailableCustomers { get; set; }
        public IEnumerable<SelectListItem>? AvailableEmployees { get; set; }

    }
}
