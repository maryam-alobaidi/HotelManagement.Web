using HotelManagement.Domain.Entities;
using HotelManagement.Infrastructure.DAL.DTOs;

namespace HotelManagement.BLL.Interfaces
{
    public interface IBookingService
    {
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<int?> AddBookingAsync(Booking booking);
        Task<bool> UpdateBookingAsync(Booking booking);
        Task<bool> DeleteBookingAsync(int id);
        Task<IEnumerable<Booking>> GetBookingsWithAllDetails();
        Task<IEnumerable<UnpaidBookingDto>> GetUnpaidBookingsAsync();
        Task<List<Booking>> GetBookingsByCustomerIdAsync(int customerId);
        Task <int>GetTotalBookingsAsync();
        Task<int> GetPendingBookingsAsync();
    }
}
