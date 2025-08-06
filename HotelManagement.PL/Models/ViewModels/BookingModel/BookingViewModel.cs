using HotelManagement.Domain.Entities;
using HotelManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Web.Models.ViewModels.BookingModel
{
    public class BookingViewModel
    {
        public int BookingID { get; set; }

        [Display(Name = "Room ID")]
        public int RoomID { get; set; }

        [Display(Name = "Customer ID")]
        public int CustomerID { get; set; }

        [Display(Name = "Check-in Date")]
        public DateTime CheckInDate { get; set; }

        [Display(Name = "Check-out Date")]
        public DateTime CheckOutDate { get; set; }

        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; }

        [Display(Name = "Number of Adults")]
        public int NumAdults { get; set; }

        [Display(Name = "Number of Children")]
        public int? NumChildren { get; set; }

        [Display(Name = "Total Price")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Booking Status")]
        public BookingStatusEnum BookingStatus { get; set; }

        [Display(Name = "Booked By Employee ID")]
        public int? BookedByEmployeeID { get; set; }


        public string? CustomerFullName { get; set; }
        // Navigation Properties
        public Employee? Employee { get; set; } 
        public Room? Room { get; set; }
        public Customer? Customer { get; set; }
    }
}
