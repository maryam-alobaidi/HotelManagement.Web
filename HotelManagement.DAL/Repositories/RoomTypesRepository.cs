using HotelManagement.Domain.Entities;
using HotelManagement.Infrastructure.DAL.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;


namespace HotelManagement.Infrastructure.DAL.Repositories
{
    public class RoomTypesRepository : IRoomTypesRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<RoomTypesRepository> _logger;
        public RoomTypesRepository(string connectionString,ILogger<RoomTypesRepository> logger)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

            _connectionString = connectionString;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");

        }


        public async Task<int?> AddAsync(RoomType roomType)
        {
            using (SqlCommand command = new SqlCommand("Sp_AddNewRoomTypes"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TypeName", roomType.TypeName);
                command.Parameters.AddWithValue("@Description", (object)roomType.Description ?? DBNull.Value);
                command.Parameters.AddWithValue("@BasePrice", roomType.BasePrice);
                command.Parameters.AddWithValue("@MaxOccupancy", (object)roomType.MaxOccupancy ?? DBNull.Value);
                command.Parameters.AddWithValue("@Amenities", (object)roomType.Amenities ?? DBNull.Value);
                command.Parameters.Add("@RoomTypeID", SqlDbType.Int).Direction = ParameterDirection.Output;


                return await PrimaryFunctions.AddAsync(command, _connectionString, "@RoomTypeID");
            }
        }

        public async Task<bool> DeleteAsync(int roomTypeID)
        {

            using (SqlCommand command = new SqlCommand("Sp_DeleteRoomTypes"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@RoomTypeID", roomTypeID);
               return await PrimaryFunctions.DeleteAsync(command, _connectionString);
            }
        }

        public async Task<bool> UpdateAsync(RoomType roomType)
        {
            RoomType? roomTypeToUpdate = await GetByIdAsync(roomType.RoomTypeID);

            if (roomTypeToUpdate == null)
            {
                throw new InvalidOperationException($"RoomType with ID {roomType.RoomTypeID} does not exist.");
            }

            using (SqlCommand command = new SqlCommand("Sp_UpdateRoomTypes"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@RoomTypeID", roomType.RoomTypeID);
                command.Parameters.AddWithValue("@TypeName", roomType.TypeName);
                command.Parameters.AddWithValue("@Description", (object)roomType.Description ?? DBNull.Value);
                command.Parameters.AddWithValue("@BasePrice", roomType.BasePrice);
                command.Parameters.AddWithValue("@MaxOccupancy", (object)roomType.MaxOccupancy ?? DBNull.Value);
                command.Parameters.AddWithValue("@Amenities", (object)roomType.Amenities ?? DBNull.Value);

                try
                {
                    await PrimaryFunctions.UpdateAsync(command, _connectionString);
                    _logger.LogInformation($"RoomType with ID {roomType.RoomTypeID} updated successfully.");

                    return true;
                }
                catch (SqlException sqlEx)
                {

                    if (sqlEx.Message.Contains("No Room Type ID found with the provided ID to update.") ||
                             sqlEx.Message.Contains("No records found to update for the provided ID."))
                    {
                        _logger.LogWarning($"Update failed for RoomType with ID {roomType.RoomTypeID}: {sqlEx.Message}");
                        return false;
                    }

                    throw;
                }
            }
        }

        public async Task<IEnumerable<RoomType>> GetAllAsync()
        {
            using (SqlCommand command = new SqlCommand("Sp_GetAllRoomTypes"))
            {
                command.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString))
                {
                    List<RoomType> roomTypes = new List<RoomType>();
                    while (reader.Read())
                    {
                        roomTypes.Add(MapToRoomType(reader));
                    }
                    return roomTypes;
                }
            }
        }

        public async Task<RoomType?> GetByIdAsync(int roomTypeID)
        {
            RoomType? roomType = null;

            if (roomTypeID <= 0) throw new ArgumentOutOfRangeException(nameof(roomTypeID));

            using (SqlCommand command = new SqlCommand("Sp_GetRoomTypesByID"))
            {
                command.Parameters.AddWithValue("@RoomTypeID", roomTypeID);
                command.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString))
                {
                    if (await reader.ReadAsync())
                    {
                        roomType=MapToRoomType(reader);
                    }
                }
            }
            return roomType;


        }

        private RoomType MapToRoomType(SqlDataReader reader)
        {
            return new RoomType
            (
                roomTypeID: reader.GetInt32(reader.GetOrdinal(nameof(RoomType.RoomTypeID))),
                typeName: reader.GetString(reader.GetOrdinal(nameof(RoomType.TypeName))),
                description: reader.IsDBNull(reader.GetOrdinal(nameof(RoomType.Description))) ? null : reader.GetString(reader.GetOrdinal(nameof(RoomType.Description))),
                basePrice: reader.GetDecimal(reader.GetOrdinal(nameof(RoomType.BasePrice))),
                 maxOccupancy: reader.IsDBNull(reader.GetOrdinal(nameof(RoomType.MaxOccupancy))) ? null : (int?)reader.GetInt32(reader.GetOrdinal(nameof(RoomType.MaxOccupancy))),
                amenities: reader.IsDBNull(reader.GetOrdinal(nameof(RoomType.Amenities))) ? null : reader.GetString(reader.GetOrdinal(nameof(RoomType.Amenities)))
                );
            
        }

        //use SelectListItem for return value and textfor drop down list in view html
        public async Task<IEnumerable<SelectListItem>> GetAllRoomTypesAsSelectListAsync()
        {
            using (SqlCommand command = new SqlCommand("Sp_GetAllRoomTypes"))
            {
                command.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString))
                {
                    List<SelectListItem> roomTypeSelectList = new List<SelectListItem>();

                    while (await reader.ReadAsync()) 
                    {
                        var roomType = MapToRoomType(reader);

                        roomTypeSelectList.Add(new SelectListItem()
                        {
                            Value = roomType.RoomTypeID.ToString(),
                            Text = roomType.TypeName
                        }
                       );
                    }
                        return roomTypeSelectList.OrderBy(rt => rt.Text).ToList();
                }
            }
        }

        public async Task<IEnumerable<RoomType>> GetAllRoomTypesAsync()
        {
            List<RoomType> roomTypes = new List<RoomType>();

            // Use your new stored procedure here
            using (SqlCommand command = new SqlCommand("Sp_GetAllRoomTypes"))
            {
                command.CommandType = CommandType.StoredProcedure; // IMPORTANT: Set CommandType to StoredProcedure

                using (SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString))
                {
                    while (await reader.ReadAsync())
                    {
                        roomTypes.Add(MapToRoomType(reader));
                    }
                }
            }
            return roomTypes;
        }


    }

}
