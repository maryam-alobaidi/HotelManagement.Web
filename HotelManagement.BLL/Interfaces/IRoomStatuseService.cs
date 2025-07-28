using HotelManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelManagement.BLL.Interfaces
{
    public interface IRoomStatuseService
    {
        Task<int?> AddNewRoomStatusAsync(RoomStatus roomStatus);
        Task<bool> UpdateRoomStatusAsync(RoomStatus roomStatus);
        Task<bool> DeleteRoomStatusAsync(int id);
        Task<RoomStatus?> GetRoomStatusByIdAsync(int id);
        Task<IEnumerable<RoomStatus>> GetAllRoomsStatusesAsync();
        Task<IEnumerable<SelectListItem>> GetAllRoomStatusesAsSelectListAsync();

    }
}
