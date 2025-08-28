using HotelManagement.BLL.Interfaces;
using HotelManagement.BLL.Services;
using HotelManagement.Domain.Entities;
using HotelManagement.Domain.Enums;
using HotelManagement.Web.Models.ViewModels.BookingModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace HotelManagement.Web.Controllers
{

    [Authorize(Roles = "Customer")]
    public class BookingsController:Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IRoomService _roomService;
        private readonly IEmployeeService _employeeService;
        private readonly ICustomerService _customerService;

        private readonly ILogger<BookingsController> _logger;

        public BookingsController(IBookingService bookingService, ILogger<BookingsController> logger, IRoomService roomService, IEmployeeService employeeService, ICustomerService customerService)
        {
            _bookingService = bookingService;
            _logger = logger;
            _roomService = roomService;
            _employeeService = employeeService;
            _customerService = customerService;
        }


        public async Task<IActionResult> MyBookings()
        {
            var email=User.Claims.FirstOrDefault(c=>c.Type==ClaimTypes.Email)?.Value;
            if(string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Email claim not found for the current user.");
                return Unauthorized("Email claim not found.");
            }

            var customer = await _customerService.GetCustomerByEmailAsync(email);
            if (customer == null)
            {
                _logger.LogWarning("Customer not found for the current user.");
                return NotFound("Customer not found.");
            }

            var allBookings = await _bookingService.GetBookingsByCustomerIdAsync(customer.CustomerID);
            var latestBooking = allBookings.OrderByDescending(b => b.BookingDate).FirstOrDefault();

            if (latestBooking == null )
            {
                _logger.LogInformation("No bookings found for customer ID {CustomerId}.", customer.CustomerID);
                return View(Enumerable.Empty<BookingViewModel>());
            }

            var bookingViewModels =new BookingViewModel
            {
                BookingID = latestBooking.BookingID,
                RoomID = latestBooking.RoomID,
                CustomerID = latestBooking.CustomerID,
                CheckInDate = latestBooking.CheckInDate,
                CheckOutDate = latestBooking.CheckOutDate,
                BookingDate = latestBooking.BookingDate,
                NumAdults = latestBooking.NumAdults,
                NumChildren = latestBooking.NumChildren,
                TotalPrice = latestBooking.TotalPrice,
                BookingStatus = latestBooking.BookingStatus,
                BookedByEmployeeID = latestBooking.BookedByEmployeeID,
                CustomerFullName = latestBooking.Customer != null ? latestBooking.Customer.FullName : null,
                Employee = latestBooking.Employee,
                Room = latestBooking.Room,
                Customer = latestBooking.Customer
            };

            return View(bookingViewModels);
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


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new BookingCreateViewModel
            {
                CheckInDate = DateTime.Now.Date,
                CheckOutDate = DateTime.Now.Date.AddDays(1)
            };

            

            viewModel.AvailableRooms = new List<SelectListItem> {
            new SelectListItem { Value = "", Text = "-- Select Dates to Load Rooms --" }
           };

           
            var customers = await _customerService.GetAllCustomersAsync();
            viewModel.AvailableCustomers = customers.Select(c => new SelectListItem
            {
                Value = c.CustomerID.ToString(),
                Text = $"{c.Firstname} {c.Lastname} ({c.Email})"
            }).ToList();

            var employees = await _employeeService.GetAllEmployeesAsync();
            viewModel.AvailableEmployees = employees.Select(e => new SelectListItem
            {
                Value = e.EmployeeID.ToString(),
                Text = $"{e.FirstName} {e.LastName} ({e.Role})"
            }).ToList();

            return View(viewModel);
        }

        public async Task<IActionResult> GetAvailableRoomsByDates(DateTime startDate, DateTime endDate)
        {
            if (startDate == default(DateTime) || endDate == default(DateTime) || startDate >= endDate)
            {
                // يمكنك إرجاع خطأ أو قائمة فارغة بناءً على كيفية التعامل مع المدخلات غير الصالحة
                return Json(new List<SelectListItem> { new SelectListItem { Value = "", Text = "Please select valid dates." } });
            }

            try
            {
                var availableRooms = await _roomService.GetAvailableRoomsAsync(startDate, endDate);

                var roomListItems = availableRooms.Select(r => new SelectListItem
                {
                    Value = r.RoomID.ToString(),
                    Text = $"{r.RoomNumber} - {r.RoomType?.TypeName ?? "Unknown Type"} (Beds: {r.BedCount}, Price: {r.PricePerNight:C})"
                }).ToList();

                // إضافة خيار افتراضي
                roomListItems.Insert(0, new SelectListItem { Value = "", Text = "-- Select a Room --" });

                return Json(roomListItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available rooms for dates {startDate} to {endDate}.", startDate, endDate);
                return StatusCode(500, "Error loading rooms.");
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingCreateViewModel model)
        {
            if (model == null)
            {
                _logger.LogWarning("Create: Booking data model was null during POST request.");
                return BadRequest("Booking data is null.");
            }


            if (model.RoomID > 0 && model.CheckOutDate != default(DateTime) && model.CheckInDate != default(DateTime))
            {
                model.TotalPrice = await _roomService.CalculateTotalPriceAsync(model.RoomID, model.CheckInDate, model.CheckOutDate, model.NumAdults, model.NumChildren);
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Model state is invalid for booking creation.");
                return View(model);
            }


            var booking = new Booking
            (
                roomID : model.RoomID,
                customerID : model.CustomerID,
                checkInDate : model.CheckInDate,
                checkOutDate : model.CheckOutDate,
                numAdults : model.NumAdults,
                numChildren : model.NumChildren,
                bookedByEmployeeID : model.BookedByEmployeeID
            );

            booking.TotalPrice = model.TotalPrice;

            try
            {
              int? bookingID = await _bookingService.AddBookingAsync(booking);
            if (bookingID>0)
            {
                _logger.LogInformation("New booking created successfully.");
                return RedirectToAction(nameof(MyBookings));
            }
            else
            {
                _logger.LogError("Create: A new booking could not be created.");
                ModelState.AddModelError("", "Failed to create booking. Please verify your input and try again.");
              
              
                return View(model);
            }
         
              
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during booking creation."); // Use ex, not ex.ToString()
                ModelState.AddModelError("", "An unexpected error occurred while adding the booking. Please try again.");
                return View(model);
            }
        }


        private async Task PopulateDropdownsAsync(BookingEditViewModel model)
        {
            // جلب قوائم العملاء والموظفين
            var customers = await _customerService.GetAllCustomersAsync();
            var employees = await _employeeService.GetAllEmployeesAsync();

            // جلب قائمة الغرف المتاحة
            var availableRooms = (await _roomService.GetAvailableRoomsAsync(model.CheckInDate, model.CheckOutDate)).ToList();

            // إضافة الغرفة الحالية إلى قائمة الغرف المتاحة للتأكد من ظهورها
            if (!availableRooms.Any(r => r.RoomID == model.RoomID))
            {
                var currentRoom = await _roomService.GetRoomByIdAsync(model.RoomID);
                if (currentRoom != null)
                {
                    availableRooms.Add(currentRoom);
                }
            }

            model.Rooms = availableRooms.Select(r => new SelectListItem
            {
                Value = r.RoomID.ToString(),
                Text = $"{r.RoomNumber} - {r.RoomType?.TypeName ?? "Unknown Type"}"
            }).ToList();

            model.Customers = customers.Select(c => new SelectListItem
            {
                Value = c.CustomerID.ToString(),
                Text = $"{c.Firstname} {c.Lastname} ({c.Email})"
            }).ToList();

            model.Employees = employees.Select(e => new SelectListItem
            {
                Value = e.EmployeeID.ToString(),
                Text = $"{e.FirstName} {e.LastName} ({e.Role})"
            }).ToList();

            model.BookingStatuses = Enum.GetValues(typeof(BookingStatusEnum))
                .Cast<BookingStatusEnum>()
                .Select(status => new SelectListItem
                {
                    Value = status.ToString(),
                    Text = status.ToString()
                }).ToList();

        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
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

            var viewModel = new BookingEditViewModel
            {
                BookingID = booking.BookingID,
                RoomID = booking.RoomID,
                CustomerID = booking.CustomerID,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                NumAdults = booking.NumAdults,
                NumChildren = booking.NumChildren,
                TotalPrice = booking.TotalPrice,
                BookingStatus = booking.BookingStatus,
                BookedByEmployeeID = booking.BookedByEmployeeID,
            };

            // Call the helper function to populate lists
            await PopulateDropdownsAsync(viewModel);

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookingEditViewModel model)
        {
            if (id <= 0)
            {
                _logger.LogError("Invalid booking ID: {Id}", id);
                return BadRequest();
            }

            if (model == null)
            {
                _logger.LogWarning("Edit: Booking data model was null during POST request.");
                return BadRequest("Booking data is null.");
            }

            if (model.RoomID > 0 && model.CheckOutDate != default(DateTime) && model.CheckInDate != default(DateTime))
            {
                model.TotalPrice = await _roomService.CalculateTotalPriceAsync(model.RoomID, model.CheckInDate, model.CheckOutDate, model.NumAdults, model.NumChildren);
            }
              

            if (!ModelState.IsValid)
            {
                _logger.LogError("Model state is invalid for booking edit.");
            
                await PopulateDropdownsAsync(model);
                return View(model);
            }

            if (id != model.BookingID)
            {
                _logger.LogError("Booking ID mismatch: {Id} does not match model BookingID {ModelId}.", id, model.BookingID);
                return BadRequest("Booking ID mismatch.");
            }

            try
            {
                var booking = new Booking
                (
                    bookingID: model.BookingID,
                    roomID: model.RoomID,
                    customerID: model.CustomerID,
                    checkInDate: model.CheckInDate,
                    checkOutDate: model.CheckOutDate,
                    bookingDate:model.CheckOutDate,
                    numAdults: model.NumAdults,
                    numChildren: model.NumChildren,
                    totalPrice: model.TotalPrice,
                    bookingStatus: model.BookingStatus,
                    bookedByEmployeeID: model.BookedByEmployeeID
                );

             

                bool isUpdated = await _bookingService.UpdateBookingAsync(booking);
                if (isUpdated)
                {
                    _logger.LogInformation("Booking with ID {Id} updated successfully.", id);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogError("Edit: Booking with ID {Id} could not be updated.", id);
                    ModelState.AddModelError("", "Failed to update booking. Please verify your input and try again.");
                    await PopulateDropdownsAsync(model); // إعادة تعبئة القوائم في حالة الفشل
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during booking edit.");
                ModelState.AddModelError("", "An unexpected error occurred while updating the booking. Please try again.");
                await PopulateDropdownsAsync(model); // إعادة تعبئة القوائم في حالة الاستثناء
                return View(model);
            }
        }



        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if(id <= 0)
            {
                _logger.LogError("Invalid booking ID: {Id}", id);
                return BadRequest();
            }

            var booking = await _bookingService.GetBookingByIdAsync(id);
            if(booking == null)
            {
                _logger.LogWarning("Booking with ID {Id} not found.", id);
                return NotFound();
            }

           var viewModel = new BookingViewModel
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
            return View(viewModel);

        }

      
        [HttpPost, ActionName("Delete")]
       
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
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

            try
            {

                bool isDeleted = await _bookingService.DeleteBookingAsync(id);
                if (isDeleted)
                {
                    _logger.LogInformation("Booking with ID {Id} deleted successfully.", id);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogError("Delete: Booking with ID {Id} could not be deleted.", id);
                    ModelState.AddModelError("", "Failed to delete booking. Please try again.");
                    return NotFound("Failed to delete booking.");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting booking with ID {Id}.", id);
                ModelState.AddModelError("", "An unexpected error occurred while deleting the booking. Please try again.");
                return BadRequest();
            }

        }
    }

}
