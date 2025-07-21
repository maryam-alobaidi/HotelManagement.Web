using HotelManagement.Domain.Entities;
using HotelManagement.Domain.Enums;
using HotelManagement.Infrastructure.DAL.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;


namespace HotelManagement.Infrastructure.DAL.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly string _connectionString;
        public BookingRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null.");
        }

        public async Task<int?> AddAsync(Booking booking)
        {
            using (SqlCommand command = new SqlCommand("Sp_AddNewBooking"))
            {
                command.CommandType = CommandType.StoredProcedure;
          
                command.Parameters.AddWithValue("@RoomID", booking.RoomID);
                command.Parameters.AddWithValue("@CustomerID", booking.CustomerID);
                command.Parameters.AddWithValue("@CheckInDate", booking.CheckInDate);
                command.Parameters.AddWithValue("@CheckOutDate", booking.CheckOutDate);
                command.Parameters.AddWithValue("@BookingDate", booking.BookingDate);
                command.Parameters.AddWithValue("@NumAdults", booking.NumAdults);
                command.Parameters.AddWithValue("@NumChildren", booking.NumChildren);
                command.Parameters.AddWithValue("@TotalPrice", booking.TotalPrice);
                command.Parameters.AddWithValue("@BookingStatus", booking.BookingStatus);
                command.Parameters.AddWithValue("@BookedByEmployeeID", booking.BookedByEmployeeID);

                command.Parameters.AddWithValue("@BookingID", SqlDbType.Int).Direction = ParameterDirection.Output;
                return await PrimaryFunctions.AddAsync(command, _connectionString, "@BookingID");
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (SqlCommand command = new SqlCommand("Sp_DeleteBooking"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@BookingID", id);
              return  await PrimaryFunctions.DeleteAsync(command, _connectionString);
            }
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            List<Booking> bookings = new List<Booking>();
            using (SqlCommand command = new SqlCommand("Sp_GetAllBooking"))
            {

                command.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString);
                while (reader.Read())
                {
                    bookings.Add(MapToBooking(reader));
                }

            }
            return bookings;
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID must be greater than zero.", nameof(id));
            }
            using (SqlCommand command = new SqlCommand("Sp_GetBookingByID"))
            {
                command.Parameters.AddWithValue("@ID", id);
                command.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString))
                {
                    if (await reader.ReadAsync())
                    {
                        return MapToBooking(reader);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public async Task<bool> UpdateAsync(Booking booking)
        {
            using (SqlCommand command = new SqlCommand("Sp_UpdateBooking"))
            {
                command.Parameters.AddWithValue("@BookingID", booking.BookingID);
                command.Parameters.AddWithValue("@RoomID", booking.RoomID);
                command.Parameters.AddWithValue("@CustomerID", booking.CustomerID);
                command.Parameters.AddWithValue("@CheckInDate", booking.CheckInDate);
                command.Parameters.AddWithValue("@CheckOutDate", booking.CheckOutDate);
                command.Parameters.AddWithValue("@BookingDate", booking.BookingDate);
                command.Parameters.AddWithValue("@NumAdults", booking.NumAdults);
                command.Parameters.AddWithValue("@NumChildren", booking.NumChildren);
                command.Parameters.AddWithValue("@TotalPrice", booking.TotalPrice);
                command.Parameters.AddWithValue("@BookingStatus", booking.BookingStatus);
                command.Parameters.AddWithValue("@BookedByEmployeeID", booking.BookedByEmployeeID);


                command.CommandType = CommandType.StoredProcedure;

                try
                {
                    await PrimaryFunctions.UpdateAsync(command, _connectionString);

                    return true;
                }
                catch (SqlException sqlEx)
                {

                    if (sqlEx.Message.Contains("No Booking ID found with the provided ID to update.") ||
                             sqlEx.Message.Contains("No records found to update for the provided ID."))
                    {
                        return false;
                    }

                    throw;
                }
            }
        }

        private Booking MapToBooking(SqlDataReader reader)
        {
            return new Booking
            (
                bookingID : reader.GetInt32(reader.GetOrdinal(nameof(Booking.BookingID))),
                roomID : reader.GetInt32(reader.GetOrdinal(nameof(Booking.RoomID))),
                customerID : reader.GetInt32(reader.GetOrdinal(nameof(Booking.CustomerID))),
                checkInDate : reader.GetDateTime(reader.GetOrdinal(nameof(Booking.CheckInDate))),
                checkOutDate: reader.GetDateTime(reader.GetOrdinal(nameof(Booking.CheckOutDate))),
                bookingDate: reader.GetDateTime(reader.GetOrdinal(nameof(Booking.BookingDate))),
                numAdults: reader.GetInt32(reader.GetOrdinal(nameof(Booking.NumAdults))),
                numChildren: reader.GetInt32(reader.GetOrdinal(nameof(Booking.NumChildren))),
                totalPrice: reader.GetDecimal(reader.GetOrdinal(nameof(Booking.TotalPrice))),
                bookingStatus: (BookingStatusEnum)reader.GetInt32(reader.GetOrdinal(nameof(Booking.BookingStatus))),
                bookedByEmployeeID: reader.GetInt32(reader.GetOrdinal(nameof(Booking.BookedByEmployeeID)))
            );
        }

        public async Task<bool> IsRoomAvailable(int roomID, DateTime checkInDate, DateTime checkOutDate, int? bookingIdToExclude = null)
        {
            using(SqlCommand command = new SqlCommand("Sp_IsRoomAvailable"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@RoomID", roomID);
                command.Parameters.AddWithValue("@CheckInDate", checkInDate);
                command.Parameters.AddWithValue("@CheckOutDate",checkOutDate);
                command.Parameters.AddWithValue("@BookingIDToExclude",bookingIdToExclude.HasValue? bookingIdToExclude.Value:DBNull.Value);

                return await PrimaryFunctions.IsRoomAvailable(command, _connectionString);
            } 
        }


    }
}
