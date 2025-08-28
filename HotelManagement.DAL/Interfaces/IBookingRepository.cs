using HotelManagement.Domain.Entities;
using HotelManagement.Infrastructure.DAL.DTOs;

namespace HotelManagement.Infrastructure.DAL.Interfaces
{
    public interface IBookingRepository
    {
        Task<int?> AddAsync(Booking booking);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateAsync(Booking booking);
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<Booking?> GetByIdAsync(int id);
        Task<bool> IsRoomAvailable(int roomID,DateTime checkInDate,DateTime checkOutDate,int? bookingIdToExclude=null);
        Task<IEnumerable<Booking>> GetBookingsWithAllDetails();
        Task<IEnumerable<UnpaidBookingDto>> GetUnpaidBookingsAsync();
        Task<List<Booking>> GetBookingsByCustomerIdAsync(int customerId);
        Task<int> GetTotalBookingsAsync();
        Task<int> GetPendingBookingsAsync();
    }
}
