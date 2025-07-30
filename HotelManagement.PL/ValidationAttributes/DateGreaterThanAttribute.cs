using System.ComponentModel.DataAnnotations;
using System.Reflection;


namespace HotelManagement.Web.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DateGreaterThanAttribute : ValidationAttribute
    {

        private readonly string _comparisonProperty;

        // Constructor: يستقبل اسم الخاصية الأخرى التي سنقارن بها
        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty ?? throw new ArgumentNullException(nameof(comparisonProperty));
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // الحصول على القيمة الحالية للخاصية التي تم تطبيق السمة عليها (أي CheckOutDate)
            if (value == null || !(value is DateTime))
            {
                // إذا كانت القيمة فارغة أو ليست تاريخاً، اترك validation آخر يتعامل معها (مثل RequiredAttribute)
                return ValidationResult.Success;
            }

            DateTime currentValue = (DateTime)value;

            // الحصول على الخاصية الأخرى التي سنقارن بها (مثل CheckInDate) باستخدام Reflection
            PropertyInfo? comparisonProperty = validationContext.ObjectType.GetProperty(_comparisonProperty);

            // التحقق مما إذا كانت الخاصية الأخرى موجودة
            if (comparisonProperty == null)
            {
                throw new ArgumentException($"Property with name '{_comparisonProperty}' not found in '{validationContext.ObjectType.Name}'.");
            }

            // الحصول على قيمة الخاصية الأخرى
            object? comparisonValueObject = comparisonProperty.GetValue(validationContext.ObjectInstance);

            // التحقق مما إذا كانت قيمة الخاصية الأخرى موجودة ونوعها تاريخ
            if (comparisonValueObject == null || !(comparisonValueObject is DateTime))
            {
                // إذا كانت قيمة الخاصية الأخرى فارغة أو ليست تاريخاً، لا يمكننا المقارنة
                return ValidationResult.Success;
            }

            DateTime comparisonValue = (DateTime)comparisonValueObject;

            // إجراء المقارنة: تاريخ المغادرة يجب أن يكون بعد تاريخ الوصول
            // نستخدم .Date للمقارنة اليوم فقط (تجاهل الوقت) لضمان إقامة لليلة واحدة على الأقل.
            if (currentValue.Date <= comparisonValue.Date)
            {
                // إذا كان تاريخ المغادرة يساوي أو قبل تاريخ الوصول، أظهر رسالة الخطأ
                // Message هو رسالة الخطأ الافتراضية، أو الرسالة التي ستمررها في السمة
                return new ValidationResult(ErrorMessage ?? $"The {_comparisonProperty.Replace("ID", " ID")} must be before the {validationContext.DisplayName.ToLower()}.", new[] { validationContext.MemberName });
            }

            // إذا كانت المقارنة صحيحة (تاريخ المغادرة بعد تاريخ الوصول)
            return ValidationResult.Success;
        }
    }
}