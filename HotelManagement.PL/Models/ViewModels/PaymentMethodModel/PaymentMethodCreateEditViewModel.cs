using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Web.Models.ViewModels.PaymentMethodModel
{
    public class PaymentMethodCreateEditViewModel
    {

        [Display(Name = "Payment Method ID")]
        public int? MethodID { get; set; }

       
        [Display(Name = "Payment Method Name")]
        [Required(ErrorMessage = "Payment Method Name is required.")]
        [MaxLength(50, ErrorMessage = "Payment Method Name cannot exceed 50 characters.")]
        public string MethodName { get; set; }


        [Display(Name = "Description")]
        [MaxLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string? Description { get; set; }


        [Display(Name = "Is Active")]
        [Required(ErrorMessage = "Please specify if the payment method is active.")]
        public bool IsActive { get; set; }

    }
}
