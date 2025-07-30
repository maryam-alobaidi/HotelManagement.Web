using HotelManagement.BLL.Interfaces;
using HotelManagement.BLL.Services;
using HotelManagement.Domain.Entities;
using HotelManagement.Web.Models.ViewModels.BookingModel;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Web.Controllers
{


    

//    HotelManagement.Infrastructure.DAL.Repositories.BookingRepository.MapToBooking(SqlDataReader reader) in BookingRepository.cs
//+
//            return new Booking
//HotelManagement.Infrastructure.DAL.Repositories.BookingRepository.GetAllAsync() in BookingRepository.cs
//+
//                    bookings.Add(MapToBooking(reader));
//HotelManagement.BLL.Services.BookingService.GetAllBookingsAsync() in BookingService.cs
//+
//           return await _bookingRepository.GetAllAsync();
//    HotelManagement.Web.Controllers.BookingsController.Index() in BookingsController.cs
//+
//            var Bookings = await _bookingService.GetAllBookingsAsync();



    public class BookingsController:Controller
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(IBookingService bookingService, ILogger<BookingsController> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var Bookings = await _bookingService.GetAllBookingsAsync();
            if (Bookings == null || !Bookings.Any()) 
            {
                _logger.LogWarning("No bookings found.");
                // Return an empty list to the view, which can display a "No bookings available" message
                return View(Enumerable.Empty<BookingViewModel>());
            }

            var bookingViewNodel=Bookings.Select(b=>new BookingViewModel
            {
                BookingID = b.BookingID,
                RoomID = b.RoomID,
                CustomerID = b.CustomerID,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                BookingDate = b.BookingDate,
                NumAdults = b.NumAdults,
                NumChildren = b.NumChildren,
                TotalPrice = b.TotalPrice,
                BookingStatus = b.BookingStatus,
                BookedByEmployeeID = b.BookedByEmployeeID,
                CustomerFullName =b.Customer !=null ?b.Customer.FullName : null,
                Employee = b.Employee,
                Room = b.Room,
                Customer = b.Customer
            }).ToList();

       
            return View(bookingViewNodel);
        }

        
        public async Task<IActionResult> Details(int id)
        {
            if(id <= 0)
            {
                _logger.LogError("Invalid booking ID: {Id}", id);
                return BadRequest();
            }

            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                _logger.LogWarning("Booking with ID {Id} not found.", id);
                return NotFound();
            }

            var bookingViewModel = new BookingViewModel
            {
                BookingID = booking.BookingID,
                RoomID = booking.RoomID,
                CustomerID = booking.CustomerID,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                BookingDate = booking.BookingDate,
                NumAdults = booking.NumAdults,
                NumChildren = booking.NumChildren,
                TotalPrice = booking.TotalPrice,
                BookingStatus = booking.BookingStatus,
                BookedByEmployeeID = booking.BookedByEmployeeID,
                CustomerFullName = booking.Customer?.FullName,
                Employee = booking.Employee,
                Room = booking.Room,
                Customer = booking.Customer
            };

            return View(bookingViewModel);
        }

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
