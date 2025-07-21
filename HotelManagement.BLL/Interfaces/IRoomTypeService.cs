using HotelManagement.Domain.Entities;


namespace HotelManagement.BLL.Interfaces
{
    public interface IRoomTypeService
    {
        Task<int?> AddNewRoomTypeAsync(RoomType roomType);
        Task<bool> UpdateRoomTypeAsync(RoomType roomType);
        Task<bool> DeleteRoomTypeAsync(int roomTypeID);
        Task<RoomType?> GetRoomTypeByIdAsync(int roomTypeID);
        Task<IEnumerable<RoomType>> GetAllRoomTypesAsync();
    }
}
