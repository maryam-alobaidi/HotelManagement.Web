using HotelManagement.Domain.Entities;


namespace HotelManagement.Infrastructure.DAL.Interfaces
{
    public interface IRoomRepository
    {
        Task<int?> AddAsync(Room room);
        Task<bool> DeleteAsync(int id);
        Task<Room?> GetByIdAsync(int id);
        Task<IEnumerable<Room>> GetAllAsync();
        Task<bool> UpdateAsync(Room room);
        Task<Room?> GetRoomByRoomNumberAsync(string RoomNumber);

        Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime startDate, DateTime endDate);
        Task<int> GetTotalRoomsAsync();
        Task<int> GetAvailableAllRoomsAsync();
    }
}
