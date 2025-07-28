using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain.Entities
{
    public class Employee
    {
        public enum EmployeeRole
        {
            Admin,
            Manager,
            Receptionist,
            Housekeeping,
            Maintenance
        }

        public int EmployeeID { get; private set; }
        public  string FirstName { get; set; }
        public  string LastName { get; set; }
        public  string Username { get; set; }
        public  string PasswordHash { get; set; }
        public  string Role { get; set; }
        public  DateTime HireDate { get; set; }

        // For creating new employees
        public Employee(string firstName, string lastName, string username, string passwordHash, string role, DateTime hireDate)
        {

            if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name cannot be null or empty.", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name cannot be null or empty.", nameof(lastName));
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username cannot be null or empty.", nameof(username));
            if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("Password hash cannot be null or empty.", nameof(passwordHash));
            if (string.IsNullOrWhiteSpace(role)) throw new ArgumentException("Role cannot be null or empty.", nameof(role));
            if (hireDate > DateTime.UtcNow) throw new ArgumentException("Hire date cannot be in the future.", nameof(hireDate)); // Using UtcNow


            FirstName = firstName;
            LastName = lastName;
            Username = username;
            PasswordHash = passwordHash;
           
            Role = role.Trim();
            HireDate = hireDate;
        }

        //For retrieving existing employees from the database
        public Employee(int employeeID, string firstName, string lastName, string username, string passwordHash, string role, DateTime hireDate)
        {
            EmployeeID = employeeID;
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            PasswordHash = passwordHash;
           
            Role = role;
            HireDate = hireDate;
        }

        private Employee()
        {
            // Parameterless constructor for ORM or serialization purposes
        }



        //public void ChangePassword(string newpassword)
        //{
        //    if(string.IsNullOrWhiteSpace(newpassword))
        //        throw new ArgumentException("Password cannot be null or empty.", nameof(newpassword));
        //    PasswordHash= newpassword;
        
        //}


        public void UpdateRole(string newRole)
        {
            if (string.IsNullOrWhiteSpace(newRole))
                throw new ArgumentException("Role cannot be null or empty.", nameof(newRole));
            Role = newRole.Trim();
        }
    }
}
