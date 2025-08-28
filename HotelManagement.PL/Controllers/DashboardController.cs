using HotelManagement.BLL.Interfaces;
using HotelManagement.Web.Models.ViewModels.DashboardModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin,Manager,Receptionist,Housekeeping,Maintenance")] // أي دور موظف
public class DashboardController : Controller
{
    private readonly IBookingService _bookingService;
    private readonly IRoomService _roomService;
    private readonly ICustomerService _customerService;

    public DashboardController(IBookingService bookingService, IRoomService roomService, ICustomerService customerService)
    {
        _bookingService = bookingService;
        _roomService = roomService;
        _customerService = customerService;
    }

    public async Task<IActionResult> Index()
    {
       
        var totalBookings = await _bookingService.GetTotalBookingsAsync();
        var pendingBookings = await _bookingService.GetPendingBookingsAsync();
        var totalRooms = await _roomService.GetTotalRoomsAsync();
        var availableRooms = await _roomService.GetAvailableAllRoomsAsync();
        var totalCustomers = await _customerService.GetTotalCustomersAsync();

        var model = new DashboardViewModel
        {
            TotalBookings = totalBookings,
            PendingBookings = pendingBookings,
            TotalRooms = totalRooms,
            AvailableRooms = availableRooms,
            TotalCustomers = totalCustomers
        };

        return View(model);
    }
}
