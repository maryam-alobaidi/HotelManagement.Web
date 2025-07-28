using HotelManagement.BLL.Interfaces;
using HotelManagement.Web.Models.ViewModels.BookingModel;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Web.Controllers
{
    public class BookingsController:Controller
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(IBookingService bookingService, ILogger<BookingsController> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

        // GET: /Bookings
        //public async Task<IActionResult> Index()
        //{
        //    // عرض قائمة بجميع الحجوزات
        //}

        //// GET: /Bookings/Details/{id}
        //public async Task<IActionResult> Details(int id)
        //{
        //    // عرض تفاصيل حجز معين
        //}

        //// GET: /Bookings/Create
        //public IActionResult Create()
        //{
        //    // عرض نموذج إنشاء حجز جديد
        //}

        //// POST: /Bookings/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(BookingCreateViewModel model)
        //{
        //    // معالجة إرسال نموذج إنشاء الحجز
        //}

        //// GET: /Bookings/Edit/{id}
        //public async Task<IActionResult> Edit(int id)
        //{
        //    // عرض نموذج تعديل حجز
        //}

        //// POST: /Bookings/Edit/{id}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, BookingEditViewModel model)
        //{
        //    // معالجة إرسال نموذج تعديل الحجز
        //}

        //// GET: /Bookings/Delete/{id}
        //public async Task<IActionResult> Delete(int id)
        //{
        //    // عرض صفحة تأكيد الحذف
        //}

        //// POST: /Bookings/Delete/{id}
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    // معالجة تأكيد الحذف
        //}
    }

}
