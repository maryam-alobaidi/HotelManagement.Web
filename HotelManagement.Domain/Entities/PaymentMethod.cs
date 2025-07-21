using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain.Entities
{
    public class PaymentMethod
    {

        public int MethodID { get; set; }
        public  string MethodName { get; set; }
        public string? Description { get; set; }

        //For adding a new payment method
        public PaymentMethod(string methodName, string? description)
        {
            MethodName = methodName ?? throw new ArgumentNullException(nameof(methodName));
            Description = description?.Trim();
        }

        //For retrieving from the database
        public PaymentMethod(int methodID, string methodName, string? description)
        {
            MethodID = methodID;
            MethodName = methodName;
            Description = description;
        }

        private PaymentMethod()
        {
        }
    }
}
