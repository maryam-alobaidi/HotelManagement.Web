
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Web.Models.ViewModels.CustomerModel
{
    public class CustomerCreateViewModel
    {
      

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        
        public string Firstname { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string Lastname { get; set; }

        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")] // Keep this for client-side validation hints
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        // Regex for 9 digits, typically starting with 6, 7, 8, or 9
        [RegularExpression(@"^[6789]\d{8}$", ErrorMessage = "Invalid Spanish phone number format. Must be 9 digits and start with 6, 7, 8, or 9.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Address")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string? Address { get; set; }

        [Display(Name = "Nationality")]
        [Required(ErrorMessage = "Nationality is required.")]
        [StringLength(50, ErrorMessage = "Nationality cannot exceed 50 characters.")]
        public string Nationality { get; set; }

        [Display(Name = "ID Number")]
        [Required(ErrorMessage = "ID number is required.")]
        [StringLength(10, ErrorMessage = "ID number cannot exceed 10 characters.")] // Max length for NIE is 9 + 1 (letter)
                                                                                    // Regex for Spanish DNI (8 digits + 1 letter) or NIE (X/Y/Z + 7 digits + 1 letter)
        [RegularExpression(@"^(\d{8}[A-HJ-NP-TV-Z]|[XYZ]\d{7}[A-HJ-NP-TV-Z])$", ErrorMessage = "Invalid Spanish DNI or NIE format.")]
        public string IDNumber { get; set; }


    }
}
