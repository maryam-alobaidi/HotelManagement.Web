using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Web.Models.ViewModels.Employee
{
    public class EmployeeEditViewModel
    {

        public int EmployeeID { get; set; } // Needed to identify which employee is being edited

        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        // Password fields are optional for edit, only if the user wants to change it
        [StringLength(100, MinimumLength = 6, ErrorMessage = "New Password must be between 6 and 100 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password (Leave blank to keep current)")]
        public string? NewPassword { get; set; } // For changing password

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "New password and confirmation password do not match.")]
        public string? ConfirmNewPassword { get; set; } // For new password confirmation

        [Required(ErrorMessage = "Employee Role is required.")]
        [Display(Name = "Employee Role")]
        public HotelManagement.Domain.Entities.Employee.EmployeeRole Role { get; set; } 


        [Required(ErrorMessage = "Hire Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; }
    }
}
