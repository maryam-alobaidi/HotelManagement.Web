using HotelManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelManagement.Infrastructure.DAL.Interfaces
{
    public interface IRoomTypesRepository
    {

        Task<int?>  AddAsync(RoomType roomType);
        Task<bool> DeleteAsync(int roomTypeID);
        Task<bool> UpdateAsync(RoomType roomType);
        Task<IEnumerable<RoomType>> GetAllAsync();
        Task<RoomType?> GetByIdAsync(int roomTypeID);
        Task<IEnumerable<SelectListItem>> GetAllRoomTypesAsSelectListAsync();
        Task<IEnumerable<RoomType>> GetAllRoomTypesAsync();
    }
}
