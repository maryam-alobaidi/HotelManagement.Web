using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain.Validation
{
    public static class Guard
    {

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;
            // Simple validation: check if it contains only digits and has a reasonable length
          return phoneNumber.All(char.IsDigit)&& phoneNumber.Length>=7 && phoneNumber.Length <= 15;
        }



    }
}
