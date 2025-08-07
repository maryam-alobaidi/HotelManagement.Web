using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Web.Models.ViewModels.RoomTypeModel
{
    public class RoomTypeCreateEditViewModel
    {

        [Display(Name = "Room Type ID")]
        public int RoomTypeID { get; set; }

      
        [Display(Name = "Room Type Name")]
        [Required(ErrorMessage = "The room type name is required.")]
        [StringLength(100, ErrorMessage = "Room type name cannot exceed 100 characters.")]
        public string TypeName { get; set; } = string.Empty;

      
        [Display(Name = "Description")]
        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string? Description { get; set; }

      
        [Display(Name = "Base Price")]
        [Required(ErrorMessage = "A base price is required.")]
        [Range(0.01, 999999.99, ErrorMessage = "Base price must be a positive value.")]
        [DataType(DataType.Currency)] // This helps browsers render the correct input type.
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal BasePrice { get; set; }

        
        [Display(Name = "Max Occupancy")]
        [Range(1, 10, ErrorMessage = "Max occupancy must be between 1 and 10.")]
        public int? MaxOccupancy { get; set; }

       
        [Display(Name = "Amenities")]
        [StringLength(500, ErrorMessage = "Amenities cannot exceed 500 characters.")]
        public string? Amenities { get; set; }
    }
}
