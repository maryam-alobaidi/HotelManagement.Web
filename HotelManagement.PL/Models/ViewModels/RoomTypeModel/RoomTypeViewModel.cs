using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Web.Models.ViewModels.RoomTypeModel
{
    public class RoomTypeViewModel
    {
        [Display(Name = "Room Type ID")]
        public int RoomTypeID { get; set; }

        [Display(Name = "Room Type Name")]
        public string TypeName { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Base Price")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal BasePrice { get; set; }

        [Display(Name = "Max Occupancy")]
        public int? MaxOccupancy { get; set; }

        [Display(Name = "Amenities")]
        public string? Amenities { get; set; }

    }
}
