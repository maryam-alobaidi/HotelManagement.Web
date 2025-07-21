using HotelManagement.Domain.Entities;

namespace HotelManagement.BLL.Interfaces
{
    public interface IRoomService
    {

        Task<int?> AddNewRoomAsync(Room room);
        Task<bool> UpdateRoomAsync(Room room);
        Task<bool> DeleteRoomAsync(int id);
        Task<Room?> GetRoomByIdAsync(int id);
        Task<IEnumerable<Room>> GetAllRoomsAsync();
        //Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime startDate, DateTime endDate);
        //Task<IEnumerable<Room>> GetRoomsByTypeAsync(int roomTypeId);
        //Task<IEnumerable<Room>> GetRoomsByStatusAsync(int roomStatusId);
        


    }
}
