using HotelManagement.BLL.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Infrastructure.DAL.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;



namespace HotelManagement.BLL.Services
{
    public class RoomStatuseService:IRoomStatuseService
    {

        private readonly IRoomStatusesRepository _roomStatusesRepository;

        public RoomStatuseService(IRoomStatusesRepository roomStatusesRepository)
        {
            _roomStatusesRepository = roomStatusesRepository ?? throw new ArgumentNullException(nameof(roomStatusesRepository));
        }

        public async Task<int?> AddNewRoomStatusAsync(RoomStatus roomStatus)
        {
            
            if (string.IsNullOrWhiteSpace(roomStatus.StatusName))
            {
                throw new ArgumentException("Room status name cannot be null or empty.", nameof(roomStatus.StatusName));
            }

            //check if the name is unique
            var existingStatus = await _roomStatusesRepository.GetByNameAsync(roomStatus.StatusName);
            if (existingStatus != null)
            {
                throw new InvalidOperationException($"A room status with the name '{roomStatus.StatusName}' already exists.");
            }


           return  await _roomStatusesRepository.AddAsync(roomStatus);
        }

        public async Task<bool> DeleteRoomStatusAsync(int id)
        {
            RoomStatus? roomStatuseToDelete = await GetRoomStatusByIdAsync(id);

            if (roomStatuseToDelete == null)
            {
                throw new KeyNotFoundException($"Room status  with ID {id} not found.");
            }
            try
            {
               return await _roomStatusesRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete room status with ID {id}.", ex);
            }
        }

        public async Task<IEnumerable<RoomStatus>> GetAllRoomsStatusesAsync()
        {
            return await _roomStatusesRepository.GetAllAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetAllRoomStatusesAsSelectListAsync()
        {
           return await _roomStatusesRepository.GetAllAsSelectListAsync();
        }

        public async Task<RoomStatus?> GetRoomStatusByIdAsync(int id)
        {
            if(id<=0)  throw new ArgumentException("ID must be greater than zero.", nameof(id));

            return await _roomStatusesRepository.GetByIdAsync(id);

        }

        public async Task<bool> UpdateRoomStatusAsync(RoomStatus roomStatus)
        {
            RoomStatus? roomStatuseToUpdate = await GetRoomStatusByIdAsync(roomStatus.RoomStatusID);

            if (roomStatuseToUpdate == null)
            {
                throw new KeyNotFoundException($"Room status  with ID {roomStatus.RoomStatusID} not found.");
            }
            try
            {
               return await _roomStatusesRepository.UpdateAsync(roomStatus);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update room status with ID {roomStatus.RoomStatusID}.", ex);
            }
        }

       
    }
}
