using HotelManagement.Domain.Entities;
namespace HotelManagement.Infrastructure.DAL.Interfaces
{
    public interface IRoomStatusesRepository
    {
        Task<int?> AddAsync(RoomStatus roomStatus);
        Task<RoomStatus?> GetByIdAsync(int id);
        Task<IEnumerable<RoomStatus>> GetAllAsync();
        Task<bool> UpdateAsync(RoomStatus roomStatus);
        Task<bool> DeleteAsync(int id);
        Task<RoomStatus?> GetByNameAsync(string statusName);

    }
}
