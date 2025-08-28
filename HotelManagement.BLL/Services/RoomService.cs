using HotelManagement.BLL.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Infrastructure.DAL.Repositories;
using HotelManagement.Infrastructure.DAL.Interfaces;
using System.Security.AccessControl;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace HotelManagement.BLL.Services;


//Make sure if the total price Calculation automatic in the create booking page,i will check in another time
public class RoomService : IRoomService
{

    private readonly IRoomRepository _roomRepository;
    private readonly IRoomTypesRepository _roomTypesRepository;
    private readonly IRoomStatusesRepository _roomStatusesRepository;
    

    public RoomService(IRoomRepository roomRepository,IRoomTypesRepository roomTypesRepository, IRoomStatusesRepository roomStatusesRepository)
    {
        _roomRepository = roomRepository;
        _roomTypesRepository = roomTypesRepository ?? throw new ArgumentNullException(nameof(roomTypesRepository));
        _roomStatusesRepository = roomStatusesRepository;
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

    public async Task<IEnumerable<SelectListItem>> GetAllRoomTypesAsSelectListItemsAsync()
    {
        // 1. Get the raw RoomType entities from the repository
        IEnumerable<RoomType> roomTypes = await _roomTypesRepository.GetAllRoomTypesAsync();

        // 2. Transform RoomType entities into SelectListItem objects
        // Order them by name for a better user experience in the dropdown
        var selectListItems = roomTypes
            .Select(rt => new SelectListItem
            {
                Value = rt.RoomTypeID.ToString(), // The ID that will be submitted
                Text = rt.TypeName                // The text displayed in the dropdown
            })
            .OrderBy(item => item.Text) // Order alphabetically by display text
            .ToList();

        return selectListItems;
    }

    public async Task<IEnumerable<SelectListItem>> GetAllRoomStatusesAsSelectListItemsAsync()
    {
        var roomStatuses = await _roomStatusesRepository.GetAllRoomStatusesAsync(); // تجلب كيانات RoomStatus
        return roomStatuses.Select(rs => new SelectListItem
        {
            Value = rs.RoomStatusID.ToString(),
            Text = rs.StatusName
        }).OrderBy(item => item.Text).ToList(); 
    }


    public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime startDate, DateTime endDate)
    {
        return await _roomRepository.GetAvailableRoomsAsync(startDate, endDate);
    }

    
    public async Task<decimal> CalculateTotalPriceAsync(int roomID, DateTime checkInDate, DateTime checkOutDate, int numAdults, int? numChildren)
    {
        // 1. Fetch room details from the repository
        Room? room = await _roomRepository.GetByIdAsync(roomID);
        if (room == null)
        {
         
            throw new KeyNotFoundException($"Room with ID {roomID} not found.");
        }

        // 2. Validate booking dates
        if (checkInDate >= checkOutDate)
        {            
            throw new ArgumentException("Check-in date must be earlier than check-out date.");
        }

        // 3. Calculate number of nights
        int numberOfNights = (checkOutDate - checkInDate).Days;

        decimal extraAdultFeePerNight = 100; 
        decimal extraChildFeePerNight = 50;

        decimal baseRoomPricePerNight = room.PricePerNight ?? 0;

        decimal totalPrice = 0;


        totalPrice+=baseRoomPricePerNight * numberOfNights;
        if (numAdults > 2) // Assuming 2 adults are included in the base price
        {
            int extraAdults = numAdults - 2;
            totalPrice += extraAdults * extraAdultFeePerNight * numberOfNights;
        }

        if (numChildren.HasValue && numChildren.Value > 0)
        {
            totalPrice += (numChildren.Value * extraChildFeePerNight) * numberOfNights;
        }

        // Ensure the total price is not negative or zero
        if (totalPrice <= 0)
        {
          
            throw new InvalidOperationException("Calculated total price must be greater than zero.");
        }

        return totalPrice;
    }

    public async Task<int> GetTotalRoomsAsync()
    {
       return await _roomRepository.GetTotalRoomsAsync();
    }

    public async Task<int> GetAvailableAllRoomsAsync()
    {
        return await _roomRepository.GetAvailableAllRoomsAsync();
    }

    //public Task<IEnumerable<Room>> GetRoomsByStatusAsync(int roomStatusId)
    //{
    //    throw new NotImplementedException();
    //}

    //public Task<IEnumerable<Room>> GetRoomsByTypeAsync(int roomTypeId)
    //{
    //    throw new NotImplementedException();
    //}




}
