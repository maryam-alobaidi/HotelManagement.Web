using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain.Enums
{
    public enum InvoiceStatusEnum
    {
        Unpaid,          // 0 - غير مدفوع
        Paid,            // 1 - مدفوع بالكامل
        PartiallyPaid,   // 2 - مدفوع جزئياً
        Overdue,         // 3 - متأخر الدفع
        Cancelled        // 4 -ملغاة
    }
}
