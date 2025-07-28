using HotelManagement.BLL.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Infrastructure.DAL.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace HotelManagement.BLL.Services
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IRoomTypesRepository _roomTypesRepository;

        public RoomTypeService(IRoomTypesRepository roomTypesRepository)
        {
            _roomTypesRepository = roomTypesRepository ?? throw new ArgumentNullException(nameof(roomTypesRepository));
        }

        public async Task<int?> AddNewRoomTypeAsync(RoomType roomType)
        {
            return await _roomTypesRepository.AddAsync(roomType);
        }

        public async Task<bool> DeleteRoomTypeAsync(int roomTypeID)
        {
            RoomType? roomTypToDelete = await GetRoomTypeByIdAsync(roomTypeID);

            if (roomTypToDelete == null)
            {
                throw new KeyNotFoundException($"Room type with ID {roomTypeID} not found.");
            }
            try
            {
                return await _roomTypesRepository.DeleteAsync(roomTypeID);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete room type with ID {roomTypeID}.", ex);
            }


        }

        public async Task<IEnumerable<RoomType>> GetAllRoomTypesAsync()
        {
            return await _roomTypesRepository.GetAllAsync();
        }

        public async Task<RoomType?> GetRoomTypeByIdAsync(int roomTypeID)
        {
            return await _roomTypesRepository.GetByIdAsync(roomTypeID);
        }

        public async Task<bool> UpdateRoomTypeAsync(RoomType roomType)
        {
            if (GetRoomTypeByIdAsync(roomType.RoomTypeID) == null)
            {
                throw new KeyNotFoundException($"Room type with ID {roomType.RoomTypeID} not found.");
            }
            try
            {
                return await _roomTypesRepository.UpdateAsync(roomType);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to Update room type with ID {roomType.RoomTypeID}.", ex);
            }
        }


        public async Task<IEnumerable<SelectListItem>> GetAllRoomTypesAsSelectListAsync()
        {
            return await _roomTypesRepository.GetAllRoomTypesAsSelectListAsync();
        }

     
    }
}