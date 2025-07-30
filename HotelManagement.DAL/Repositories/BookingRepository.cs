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
                command.Parameters.AddWithValue("@BookingID", id);
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
            // Ensure all GetOrdinal calls are optimized by storing ordinals
            // Example: int roomNumberOrdinal = reader.GetOrdinal(nameof(Booking.Room.RoomNumber));

            //read all the data of room
            var room = new Room
            (
                roomID: reader.GetInt32(reader.GetOrdinal(nameof(Booking.RoomID))),
                roomNumber: reader.GetString(reader.GetOrdinal(nameof(Booking.Room.RoomNumber))), // 'RoomNumber' is likely direct column name
                bedCount: reader.GetInt32(reader.GetOrdinal(nameof(Booking.Room.BedCount))),      // 'BedCount' is likely direct column name
                roomTypeID: reader.GetInt32(reader.GetOrdinal(nameof(Booking.Room.RoomTypeID))),  // 'RoomTypeID' is likely direct column name
                pricePerNight: reader.IsDBNull(reader.GetOrdinal(nameof(Booking.Room.PricePerNight))) ? null : reader.GetDecimal(reader.GetOrdinal(nameof(Booking.Room.PricePerNight))),
                roomStatusID: reader.GetInt32(reader.GetOrdinal(nameof(Booking.Room.RoomStatusID)))
            );

            //read all the data of customer
            var customer = new Customer
            (
                customerID: reader.GetInt32(reader.GetOrdinal(nameof(Booking.CustomerID))), // This is from Booking.CustomerID, not Customer.CustomerID
                                                                                            // CORRECTED: Use alias 'CustomerFirstName'
                firstname: reader.GetString(reader.GetOrdinal("CustomerFirstName")),
                // CORRECTED: Use alias 'CustomerLastName'
                lastname: reader.GetString(reader.GetOrdinal("CustomerLastName")),
                email: reader.GetString(reader.GetOrdinal(nameof(Booking.Customer.Email))),
                phoneNumber: reader.GetString(reader.GetOrdinal(nameof(Booking.Customer.PhoneNumber))),
                address: reader.IsDBNull(reader.GetOrdinal(nameof(Booking.Customer.Address))) ? null : reader.GetString(reader.GetOrdinal(nameof(Booking.Customer.Address))),
                nationality: reader.GetString(reader.GetOrdinal(nameof(Booking.Customer.Nationality))),
                iDNumber: reader.GetString(reader.GetOrdinal(nameof(Booking.Customer.IDNumber)))
            );

            Employee? employee = null;
            // CORRECTED: Use alias 'EmployeeFirstName'
            int employeeFirstNameOrdinal = reader.GetOrdinal("EmployeeFirstName");
            if (!reader.IsDBNull(employeeFirstNameOrdinal))
            {
                employee = new Employee(
                    employeeID: reader.GetInt32(reader.GetOrdinal(nameof(Booking.BookedByEmployeeID))), // EmployeeID should map from BookedByEmployeeID
                                                                                                        // CORRECTED: Use alias 'EmployeeFirstName'
                    firstName: reader.GetString(reader.GetOrdinal("EmployeeFirstName")),
                    // CORRECTED: Use alias 'EmployeeLastName'
                    lastName: reader.GetString(reader.GetOrdinal("EmployeeLastName")),
                    username: reader.GetString(reader.GetOrdinal(nameof(Booking.Employee.Username))),
                    passwordHash: reader.GetString(reader.GetOrdinal(nameof(Booking.Employee.PasswordHash))),
                    role: reader.GetString(reader.GetOrdinal(nameof(Booking.Employee.Role))),
                    hireDate: reader.GetDateTime(reader.GetOrdinal(nameof(Booking.Employee.HireDate)))
                );
            }

            // Creating Booking object (this part seems fine if you adjusted to use ordinals as suggested previously)
            var booking = new Booking
            (
                bookingID: reader.GetInt32(reader.GetOrdinal(nameof(Booking.BookingID))),
                roomID: reader.GetInt32(reader.GetOrdinal(nameof(Booking.RoomID))),
                customerID: reader.GetInt32(reader.GetOrdinal(nameof(Booking.CustomerID))),
                checkInDate: reader.GetDateTime(reader.GetOrdinal(nameof(Booking.CheckInDate))),
                checkOutDate: reader.GetDateTime(reader.GetOrdinal(nameof(Booking.CheckOutDate))),
                bookingDate: reader.GetDateTime(reader.GetOrdinal(nameof(Booking.BookingDate))),
                numAdults: reader.GetInt32(reader.GetOrdinal(nameof(Booking.NumAdults))),
                numChildren: reader.IsDBNull(reader.GetOrdinal(nameof(Booking.NumChildren))) ? (int?)null : reader.GetInt32(reader.GetOrdinal(nameof(Booking.NumChildren))),
                totalPrice: reader.GetDecimal(reader.GetOrdinal(nameof(Booking.TotalPrice))),
                bookingStatus: (BookingStatusEnum)reader.GetInt32(reader.GetOrdinal(nameof(Booking.BookingStatus))),
                bookedByEmployeeID: reader.IsDBNull(reader.GetOrdinal(nameof(Booking.BookedByEmployeeID))) ? (int?)null : reader.GetInt32(reader.GetOrdinal(nameof(Booking.BookedByEmployeeID)))
            );

            booking.Room = room;
            booking.Customer = customer;
            booking.Employee = employee;

            return booking;
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
