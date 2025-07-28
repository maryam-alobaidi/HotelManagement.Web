
namespace HotelManagement.BLL.Interfaces
{
    public interface IPasswordHasherService
    {
        //This interface defines methods for hashing and verifying passwords.


        // هذه الدالة ستُرجع الـ hash فقط، لأن الـ salt يكون "مدمجًا" داخل الـ hash في BCrypt
        string HashPassword(string password);

        // هذه الدالة ستقارن كلمة المرور المدخلة بالهاش المخزن
        bool VerifyPassword(string password, string hashedPassword);

    }
}
