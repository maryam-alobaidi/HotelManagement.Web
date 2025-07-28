using HotelManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Web.Models.ViewModels.RoomModel
{
    public class RoomCreateViewModel
    {
      
        [Required(ErrorMessage = "Room Number is required.")]
        [StringLength(10, ErrorMessage = "Room Number cannot exceed 10 characters.")]
        [Display(Name = "Room Number")]
        public string RoomNumber { get; set; }

        [Required(ErrorMessage = "Bed Count is required.")]
        [Range(1, 10, ErrorMessage = "Bed Count must be between 1 and 10.")] 
        [Display(Name = "Number of Beds")]
        public int BedCount { get; set; }

        [Required(ErrorMessage = "Room Type is required.")]
        [Display(Name = "Room Type")]
        public int RoomTypeID { get; set; } 

        [Display(Name = "Price Per Night")]
        [DataType(DataType.Currency)]
      
        [Range(0.01, 10000.00, ErrorMessage = "Price must be greater than 0 and less than 10000.")]
        public decimal? PricePerNight { get; set; } 

        [Required(ErrorMessage = "Room Status is required.")]
        [Display(Name = "Room Status")]
        public int RoomStatusID { get; set; }

        // These properties are for populating the dropdowns in the GET Create action
        public IEnumerable<SelectListItem>? RoomTypes { get; set; }
        public IEnumerable<SelectListItem>? RoomStatuses { get; set; }
    }
}
