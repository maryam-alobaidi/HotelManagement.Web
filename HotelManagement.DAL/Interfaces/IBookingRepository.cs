using HotelManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
