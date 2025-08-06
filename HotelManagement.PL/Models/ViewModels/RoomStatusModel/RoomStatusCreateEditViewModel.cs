using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Web.Models.ViewModels.RoomStatusModel
{
    public class RoomStatusCreateEditViewModel
    {
        public int RoomStatusID { get; set; } = 0;

        [Required(ErrorMessage = "The status name is required.")]
        [StringLength(50, ErrorMessage = "Status name cannot exceed 50 characters.")]
        [Display(Name = "Status Name")]
        public string StatusName { get; set; }

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}
