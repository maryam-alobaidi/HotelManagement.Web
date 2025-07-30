using System.ComponentModel.DataAnnotations;


namespace HotelManagement.Web.Models.ViewModels.EmployeeModel
{
    public class EmployeeViewModel
    {
       
        public int? EmployeeID { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [Display(Name = "Username")]
        public string Username { get; set; }

       
       
        [Display(Name = "Employee Role")]
        public HotelManagement.Domain.Entities.Employee.EmployeeRole Role { get; set; } // Use uppercase 'R' for consistency


        [Display(Name = "Hire Date")]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        public string Fullname => $"{FirstName} {LastName}";
    }
}
