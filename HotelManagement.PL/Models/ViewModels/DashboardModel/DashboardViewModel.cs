namespace HotelManagement.Web.Models.ViewModels.DashboardModel
{
    public class DashboardViewModel
    {
        public int TotalBookings { get; set; }
        public int PendingBookings { get; set; }
        public int TotalRooms { get; set; }
        public int AvailableRooms { get; set; }
        public int TotalCustomers { get; set; }
    }
}
