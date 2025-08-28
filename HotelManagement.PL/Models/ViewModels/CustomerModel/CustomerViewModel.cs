using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Web.Models.ViewModels.CustomerModel
{
    public class CustomerViewModel
    {
    
        public int CustomerID { get; set; }

        [Display(Name = "First Name")]
        public string Firstname { get; set; }

        
        [Display(Name = "Last Name")]
        public string Lastname { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

       
        [Display(Name = "Address")]
        public string? Address { get; set; }

      
        [Display(Name = "Nationality")]
        public string Nationality { get; set; }


        [Display(Name = "ID Number")]
        public string IDNumber { get; set; }

        public string FullName => $"{Firstname} {Lastname}";
    }
}
