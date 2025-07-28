using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Web.Models.ViewModels.RoomModel
{
    public class RoomEditViewModel
    {
        public int RoomID { get; set; }

        [Required(ErrorMessage = "Room number is required.")]
        [StringLength(10, ErrorMessage = "Room number cannot exceed 10 characters.")]
        [Display(Name = "Room Number")]
        public string RoomNumber { get; set; }

        [Required(ErrorMessage = "Bed count is required.")]
        [Range(1, 10, ErrorMessage = "Bed count must be between 1 and 10.")]
        [Display(Name = "Number of Beds")]
        public int BedCount { get; set; }

        [Required(ErrorMessage = "Room Type is required.")] 
        [Display(Name = "Room Type")]
        public int RoomTypeID { get; set; } 

        public IEnumerable<SelectListItem>? RoomTypes { get; set; }


        [Required(ErrorMessage = "Price per night is required.")]
        [Range(0.01, 1000.00, ErrorMessage = "Price must be between 0.01 and 1000.00.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Price Per Night")]
        public decimal PricePerNight { get; set; } 

        [Required(ErrorMessage = "Room Status is required.")] 
        [Display(Name = "Room Status")]
        public int RoomStatusID { get; set; }

        public IEnumerable<SelectListItem>? RoomStatuses { get; set; }
    }
}