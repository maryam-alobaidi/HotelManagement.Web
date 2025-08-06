using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Web.Models.ViewModels.RoomStatusModel
{
    public class RoomStatusViewModel
    {

        public int RoomStatusID { get; set; }

        [Display(Name = "Status Name")]
        public string StatusName { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

    }
}
