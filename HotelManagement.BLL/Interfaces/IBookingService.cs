using HotelManagement.Domain.Entities;

namespace HotelManagement.BLL.Interfaces
{
    public interface IBookingService
    {
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<int?> AddBookingAsync(Booking booking);
        Task<bool> UpdateBookingAsync(Booking booking);
        Task<bool> DeleteBookingAsync(int id);

    }
}
