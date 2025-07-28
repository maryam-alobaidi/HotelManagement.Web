using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Web.Models.ViewModels.Employee
{
    public class EmployeeCreateViewModel
    {

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

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")] // Reasonable length for plain text password
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }// For plan password input

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } // For password confirmation

        [Required(ErrorMessage = "Employee Role is required.")]
        [Display(Name = "Employee Role")]
        public HotelManagement.Domain.Entities.Employee.EmployeeRole Role { get; set; } // Use uppercase 'R' for consistency


        [Required(ErrorMessage = "Hire Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; }
    }
}
