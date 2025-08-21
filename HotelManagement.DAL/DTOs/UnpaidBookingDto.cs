
namespace HotelManagement.Infrastructure.DAL.DTOs
{
    public class UnpaidBookingDto
    {
        public int BookingID { get; set; }
        public int CustomerID { get; set; } 
        public string FullName { get; set; }
        public string RoomNumber { get; set; }
        public DateTime CheckInDate { get; set; }
        public int InvoiceID { get; set; }
    }
}
