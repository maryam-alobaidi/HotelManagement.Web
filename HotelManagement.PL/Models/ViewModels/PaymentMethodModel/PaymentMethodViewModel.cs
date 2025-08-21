using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Web.Models.ViewModels.PaymentMethodModel
{
    public class PaymentMethodViewModel
    {
        [Display(Name = "Payment Method ID")]
        public int MethodID { get; set; }

       
        [Display(Name = "Payment Method Name")]
        public string MethodName { get; set; }


        [Display(Name = "Description")]
        public string? Description { get; set; }


        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }


    }
}
