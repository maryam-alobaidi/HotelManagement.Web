using HotelManagement.Infrastructure.DAL.Interfaces;
using HotelManagement.Domain.Entities;
using Microsoft.Data.SqlClient;
using HotelManagement.Infrastructure.DAL.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelManagement.Infrastructure.DAL.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly string _connectionString;

        public RoomRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

            _connectionString = connectionString;

        }
        public async Task<int?> AddAsync(Room room)
        {
            using(SqlCommand command = new SqlCommand("Sp_AddNewRoom"))
            {
              
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);
                command.Parameters.AddWithValue("@BedCount", room.BedCount);
                command.Parameters.AddWithValue("@RoomTypeID", room.RoomTypeID);
                command.Parameters.AddWithValue("@PricePerNight", room.PricePerNight);
                command.Parameters.AddWithValue("@RoomStatusID", room.RoomStatusID);
                command.Parameters.Add("@RoomID", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.Output;
                return  await PrimaryFunctions.AddAsync(command, _connectionString, "@RoomID");

            }
        }
        public async Task<bool> DeleteAsync(int id)
        {
            using (SqlCommand command = new SqlCommand("Sp_DeleteRoom"))
            {
                command.Parameters.AddWithValue("@RoomID", id);
                command.CommandType = System.Data.CommandType.StoredProcedure;

               return await PrimaryFunctions.DeleteAsync(command, _connectionString);
            }
        }
        public async Task<bool>  UpdateAsync(Room room)
        {
            using (SqlCommand command = new SqlCommand("Sp_UpdateRoom"))
            {
                command.Parameters.AddWithValue("@RoomID", room.RoomID);
                command.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);
                command.Parameters.AddWithValue("@BedCount", room.BedCount);
                command.Parameters.AddWithValue("@RoomTypeID", room.RoomTypeID);
                command.Parameters.AddWithValue("@PricePerNight", room.PricePerNight);
                command.Parameters.AddWithValue("@RoomStatusID", room.RoomStatusID);

                command.CommandType = System.Data.CommandType.StoredProcedure;


                try
                {
                    await PrimaryFunctions.UpdateAsync(command, _connectionString);

                    return true;
                }
                catch (SqlException sqlEx)
                {

                    if (sqlEx.Message.Contains("No Room ID found with the provided ID to update.") ||
                             sqlEx.Message.Contains("No records found to update for the provided ID."))
                    {
                        return false;
                    }

                    throw;
                }
            }
        }
        public async Task<IEnumerable<Room>> GetAllAsync()
        {
            List<Room> rooms = new List<Room>();
            using (SqlCommand command=new SqlCommand("Sp_GetAllRoom"))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                using (SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString))
                { 
                    while (await reader.ReadAsync())
                    {
                        rooms.Add(MapToRoom(reader));
                    }
                }

               return rooms;
            } 
        }
        public async Task<Room?> GetByIdAsync(int id)
        {
            if(id <= 0) throw new ArgumentException("ID must be greater than zero.", nameof(id));
            using (SqlCommand command = new SqlCommand("Sp_GetRoomById"))
            {
                command.Parameters.AddWithValue("@RoomID", id);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                using (SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString))
                {
                    if (await reader.ReadAsync())
                    {
                        return MapToRoom(reader);
                    }
                    else
                    {
                        return null; 
                    }

                }
            }
        }
        private Room MapToRoom(SqlDataReader reader)
        {
            // First, create the Room object using your existing constructor
            var room = new Room(
                roomID: reader.GetInt32(reader.GetOrdinal(nameof(Room.RoomID))),
                roomNumber: reader.GetString(reader.GetOrdinal(nameof(Room.RoomNumber))),
                bedCount: reader.GetInt32(reader.GetOrdinal(nameof(Room.BedCount))),
                roomTypeID: reader.GetInt32(reader.GetOrdinal(nameof(Room.RoomTypeID))),
                pricePerNight: reader.IsDBNull(reader.GetOrdinal(nameof(Room.PricePerNight))) ? null : reader.GetDecimal(reader.GetOrdinal(nameof(Room.PricePerNight))),
                roomStatusID: reader.GetInt32(reader.GetOrdinal(nameof(Room.RoomStatusID)))
            );

            // Now, populate the navigation properties with the data from the JOINs
            // Ensure RoomType and RoomStatus are not null before accessing (if they were not initialized in the constructor)
            // Since you initialize them in your constructor, they won't be null here.
            room.RoomType.TypeName = reader.GetString(reader.GetOrdinal("TypeName"));
            room.RoomStatus.StatusName = reader.GetString(reader.GetOrdinal("StatusName"));

            return room;
        }
        public async Task<Room?> GetRoomByRoomNumberAsync(string RoomNumber)
        {
            if (string.IsNullOrWhiteSpace(RoomNumber))
               return null;

            using (SqlCommand command = new SqlCommand("Sp_GetRoomByRoomNumber"))
            {
                command.Parameters.AddWithValue("@RoomNumber", RoomNumber);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                using (SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString))
                {
                    if (await reader.ReadAsync())
                    {
                        return MapToRoom(reader);
                    }
                    else
                    {
                        return null; // No room found with the given RoomNumber
                    }
                }
            }

        }



     

    }
}
