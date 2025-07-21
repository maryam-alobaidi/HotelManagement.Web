using HotelManagement.BLL.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Infrastructure.DAL.Repositories;
using HotelManagement.Infrastructure.DAL.Interfaces;
using System.Security.AccessControl;
namespace HotelManagement.BLL.Services;

public class RoomService : IRoomService
{

    private readonly IRoomRepository _roomRepository;

    public RoomService(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task<int?> AddNewRoomAsync(Room room)
    {
        var existingRoom = await _roomRepository.GetRoomByRoomNumberAsync(room.RoomNumber);
        if (existingRoom != null)
        {
            throw new InvalidOperationException($"The number of room '{room.RoomNumber}' Already exists.");
        }

      return  await _roomRepository.AddAsync(room);
    }

    public async Task<bool> DeleteRoomAsync(int id)
    {
      
        Room? roomToDelete =await GetRoomByIdAsync(id);

        if (roomToDelete == null)
        {
            throw new KeyNotFoundException($"Room with ID {id} not found.");
        }
        try
        {
         return   await _roomRepository.DeleteAsync(id);
        }
        catch(Exception ex)
        {
            throw new Exception($"Failed to delete room with ID {id}.",ex);
        }
    }

    public async Task<bool> UpdateRoomAsync(Room room)
    {
        if(GetRoomByIdAsync(room.RoomID) == null)
        {
            throw new KeyNotFoundException($"Room with ID {room.RoomID} not found.");
        }
        try
        {
          return  await _roomRepository.UpdateAsync(room);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to Update room with ID {room.RoomID}.", ex);
        }
       
    }

    public async Task<Room?> GetRoomByIdAsync(int id)
    {
        return await _roomRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Room>> GetAllRoomsAsync()
    {
       return await _roomRepository.GetAllAsync();
    }

    //public Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime startDate, DateTime endDate)
    //{
    //    throw new NotImplementedException();
    //}

    //public Task<IEnumerable<Room>> GetRoomsByStatusAsync(int roomStatusId)
    //{
    //    throw new NotImplementedException();
    //}

    //public Task<IEnumerable<Room>> GetRoomsByTypeAsync(int roomTypeId)
    //{
    //    throw new NotImplementedException();
    //}

  


}
