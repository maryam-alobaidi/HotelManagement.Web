using HotelManagement.Domain.Validation;

namespace HotelManagement.Domain.Entities
{
    public class Customer
    {
        public int CustomerID { get;private set; }
        public  string Firstname { get; private set; }
        public  string Lastname { get; private set; }
        public  string Email { get; private set; }
        public  string PhoneNumber { get; private set; }
        public string? Address { get; private set; }
        public  string Nationality { get; private set; }
        public  string IDNumber { get; private set; }

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
