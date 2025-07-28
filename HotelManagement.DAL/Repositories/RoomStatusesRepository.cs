using HotelManagement.Domain.Entities;
using HotelManagement.Infrastructure.DAL.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace HotelManagement.Infrastructure.DAL.Repositories
{
    public class RoomStatusesRepository : IRoomStatusesRepository
    {

        private readonly string _connectionString;

        public RoomStatusesRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<int?> AddAsync(RoomStatus roomStatus)
        {
            
            using (SqlCommand command = new SqlCommand("Sp_AddNewRoomStatuses"))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@StatusName", roomStatus.StatusName);
                command.Parameters.AddWithValue("@Description", (object)roomStatus.Description??DBNull.Value);
                command.Parameters.Add("@RoomStatusID", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.Output;
             
                command.Parameters.Add("@RoomStatusID", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.Output;

                return await PrimaryFunctions.AddAsync(command, _connectionString, "@RoomStatusID");
            } 
        }
        public async Task<bool> DeleteAsync(int id)
        {
            using (SqlCommand command = new SqlCommand("Sp_DeleteRoomStatuses"))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@RoomStatusID", id);
              return  await PrimaryFunctions.DeleteAsync(command, _connectionString);
            }
        }

        public async Task<IEnumerable<SelectListItem>> GetAllAsSelectListAsync()
        {
       
            using (SqlCommand command = new SqlCommand("Sp_GetAllRoomStatuses"))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                using (SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString))
                {
                    List<SelectListItem> selectListItems = new List<SelectListItem>();

                    while (await reader.ReadAsync())
                    {
                        var roomStatus = MapToRoomStatuse(reader);

                        selectListItems.Add(new SelectListItem
                        {
                            Value = roomStatus.RoomStatusID.ToString(),
                            Text = roomStatus.StatusName ?? string.Empty
                        });
                    }
                    return selectListItems.OrderBy(item => item.Text).ToList();
                }
          
            }
        }
        public async Task<IEnumerable<RoomStatus>> GetAllAsync()
        {
            List<RoomStatus> roomStatuses = new List<RoomStatus>();
            using (SqlCommand command = new SqlCommand("Sp_GetAllRoomStatuses"))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                using (SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString))
                {
                    while ( await reader.ReadAsync())
                    {
                        roomStatuses.Add(MapToRoomStatuse(reader)); 
                    }
                }
                return roomStatuses;
            }
        }
        public async Task<RoomStatus?> GetByIdAsync(int id)
        {
            using (SqlCommand command = new SqlCommand("Sp_GetRoomStatusesByID"))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@RoomStatusID", id);
                using (SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString))
                {
                    if (await reader.ReadAsync())
                    {
                        return MapToRoomStatuse(reader);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
        public async Task<RoomStatus?> GetByNameAsync(string statusName)
        {
            using (SqlCommand command = new SqlCommand("Sp_GetRoomStatusesByStatusName"))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@StatusName", statusName);
                using (SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString))
                {
                    if (await reader.ReadAsync())
                    {
                        return MapToRoomStatuse(reader);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
        public async Task<bool> UpdateAsync(RoomStatus roomStatus)
        {
            using (SqlCommand command = new SqlCommand("Sp_UpdateRoomStatuses"))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@RoomStatusID", roomStatus.RoomStatusID);
                command.Parameters.AddWithValue("@StatusName", roomStatus.StatusName);
                command.Parameters.AddWithValue("@Description", (object)roomStatus.Description ?? DBNull.Value);


                try
                {
                    await PrimaryFunctions.UpdateAsync(command, _connectionString);

                    return true;
                }
                catch (SqlException sqlEx)
                {

                    if (sqlEx.Message.Contains("No Room Status ID found with the provided ID to update.") ||
                             sqlEx.Message.Contains("No records found to update for the provided ID."))
                    {
                        return false;
                    }

                    throw;
                }
            }
        }
        private RoomStatus MapToRoomStatuse(SqlDataReader reader)
        {
            return new RoomStatus
           (
                 roomStatusID: reader.GetInt32(reader.GetOrdinal(nameof(RoomStatus.RoomStatusID))), // Use nameof for safety  
                 statusName: reader.IsDBNull(reader.GetOrdinal(nameof(RoomStatus.StatusName))) ? null : reader.GetString(reader.GetOrdinal(nameof(RoomStatus.StatusName))),
                 description: reader.IsDBNull(reader.GetOrdinal(nameof(RoomStatus.Description))) ? null : reader.GetString(reader.GetOrdinal(nameof(RoomStatus.Description)))
                
           );
        }


        public async Task<IEnumerable<RoomStatus>> GetAllRoomStatusesAsync()
        {
            List<RoomStatus> roomStatuses = new List<RoomStatus>();

            // استخدام الإجراء المخزن Sp_GetAllRoomStatuses
            using (SqlCommand command = new SqlCommand("Sp_GetAllRoomStatuses"))
            {
                command.CommandType = CommandType.StoredProcedure; // مهم: تحديد نوع الأمر كإجراء مخزن

              
                using (SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString))
                {
                    while (await reader.ReadAsync())
                    {
                        roomStatuses.Add(MapToRoomStatuse(reader));
                    }
                }
            }
            return roomStatuses;
        }
    }
}
