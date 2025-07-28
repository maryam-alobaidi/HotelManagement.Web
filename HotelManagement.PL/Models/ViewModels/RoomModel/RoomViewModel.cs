
using System.ComponentModel.DataAnnotations; 

namespace HotelManagement.Web.Models.ViewModels.RoomModel
{
    public class RoomViewModel
    {
        [Display(Name = "Room ID")] 
        public int RoomID { get; set; }

        [Display(Name = "Room Number")]
        public string RoomNumber { get; set; }

        [Display(Name = "Bed Count")]
        public int BedCount { get; set; }

        [Display(Name = "Room Type")] 
        public string? RoomTypeName { get; set; }

        [Display(Name = "Price Per Night")]
        [DataType(DataType.Currency)] 
        public decimal PricePerNight { get; set; }

        [Display(Name = "Status")] 
        public string? RoomStatusName { get; set; }
    }
}