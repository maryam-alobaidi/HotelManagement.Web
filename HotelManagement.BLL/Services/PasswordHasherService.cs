using HotelManagement.BLL.Interfaces;
using Org.BouncyCastle.Crypto.Generators;


namespace HotelManagement.BLL.Services
{
    public class PasswordHasherService : IPasswordHasherService
    {
        public string HashPassword(string password)
        {
            // هذه هي الطريقة الصحيحة لتشفير كلمة المرور باستخدام BCrypt.Net-Core.
            // المكتبة تتولى كل شيء:
            // 1. توليد salt عشوائي آمن لكل عملية (لا تحتاجين لإنشاء الـ salt يدوياً).
            // 2. دمج الـ salt مع كلمة المرور.
            // 3. تطبيق خوارزمية التجزئة (BCrypt) مع عامل التكلفة (cost factor) الافتراضي (عادةً 10 أو 12).
            // 4. إرجاع سلسلة نصية واحدة (string) تحتوي على الـ salt ومعلومات التكلفة والهاش النهائي.
            //    هذه السلسلة هي ما يجب تخزينه في قاعدة البيانات.
            return BCrypt.Net.BCrypt.HashPassword(password);
        }


        public bool VerifyPassword(string password, string hashedPassword)
        {
            // هذه هي الطريقة الصحيحة للتحقق من كلمة المرور.
            // المكتبة تتولى:
            // 1. استخراج الـ salt وعامل التكلفة من الـ hashedPassword المخزن.
            // 2. إعادة تشفير كلمة المرور المدخلة بنفس الـ salt وعامل التكلفة.
            // 3. مقارنة الهاش الجديد بالـ hashedPassword المخزن.
           return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
