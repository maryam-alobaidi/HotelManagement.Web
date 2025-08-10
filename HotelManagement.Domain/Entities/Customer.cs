using HotelManagement.Domain.Validation;

namespace HotelManagement.Domain.Entities
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public  string Firstname { get;  set; }
        public  string Lastname { get;  set; }
        public  string Email { get;  set; }
        public  string PhoneNumber { get;  set; }
        public string? Address { get;  set; }
        public  string Nationality { get;  set; }
        public  string IDNumber { get;  set; }

        public string FullName => $"{Firstname} {Lastname}"; // Property to get full name of the customer

        //for creare new customer
        public Customer(string firstname, string lastname, string email, string phoneNumber, string? address, string nationality, string iDNumber)
        {
            Firstname = firstname ?? throw new ArgumentNullException(nameof(firstname));
            Lastname = lastname ?? throw new ArgumentNullException(nameof(lastname));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
            Address = address;
            Nationality = nationality ?? throw new ArgumentNullException(nameof(nationality));
            IDNumber = iDNumber ?? throw new ArgumentNullException(nameof(iDNumber));
        }


        //for return data from database 
        public Customer(int customerID, string firstname, string lastname, string email, string phoneNumber, string? address, string nationality, string iDNumber)
        {
            CustomerID = customerID;
            Firstname = firstname;
            Lastname = lastname;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            Nationality = nationality;
            IDNumber = iDNumber;
        } 
        public Customer()
        {
        }

        public void UpdateContactInfo(string? email, string? phoneNumber, string? address)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                if (Guard.IsValidEmail(email))
                    Email = email.Trim();
            }
            else
            {
                throw new ArgumentException("Email not Valid.", nameof(email));
            }


            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                if (Guard.IsValidPhoneNumber(phoneNumber))
                    PhoneNumber = phoneNumber.Trim();
            }
            else
            {
                
                    throw new ArgumentException("phoneNumber not Valid.", nameof(phoneNumber));
                
            }

            if (address!=null)
            {
                Address=string.IsNullOrWhiteSpace(address)?null:address;
            }

        }


    }
}
