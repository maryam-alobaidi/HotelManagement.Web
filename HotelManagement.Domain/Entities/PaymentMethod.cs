
namespace HotelManagement.Domain.Entities
{
    public class PaymentMethod
    {

        public int MethodID { get; set; }
        public  string MethodName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        //For adding a new payment method
        public PaymentMethod(string methodName, string? description, bool isActive)
        {
            MethodName = methodName ?? throw new ArgumentNullException(nameof(methodName));
            Description = description?.Trim();
            IsActive = isActive;
        }

        //For retrieving from the database
        public PaymentMethod(int methodID, string methodName, string? description, bool isActive)
        {
            MethodID = methodID;
            MethodName = methodName;
            Description = description;
            IsActive = isActive;
        }

        public PaymentMethod()
        {
        }
    }
}
