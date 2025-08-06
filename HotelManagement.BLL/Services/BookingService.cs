using HotelManagement.BLL.Interfaces;
using HotelManagement.DAL.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Infrastructure.DAL.Interfaces;


namespace HotelManagement.BLL.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public BookingService(IBookingRepository bookingRepository, ICustomerRepository customerRepository, IRoomRepository roomRepository, IEmployeeRepository employeeRepository)
        {
            _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
        }

        public async Task<int?> AddBookingAsync(Booking booking)
        {
            Room? room = await _roomRepository.GetByIdAsync(booking.RoomID);
            if (room == null) throw new KeyNotFoundException("Room Id not found.");
            
            Customer? customer = await _customerRepository.GetByIdAsync(booking.CustomerID);
            if(customer == null) throw new KeyNotFoundException("Customer not found."); 

            if(booking.BookedByEmployeeID.HasValue)
            {
                Employee? employee = await _employeeRepository.GetByIdAsync(booking.BookedByEmployeeID.Value);
                if (employee == null) throw new KeyNotFoundException("Employee not found.");
            }


            //// at here the  problem is that the booking is not calculated with the room price
            //bool isRoomAvailable = await _bookingRepository.IsRoomAvailable(booking.RoomID, booking.CheckInDate, booking.CheckOutDate);

            //if(!isRoomAvailable) throw new InvalidOperationException("Room is not available for the selected dates.");

            return await _bookingRepository.AddAsync(booking);
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Booking ID must be greater than zero.", nameof(id));
            }
          return  await _bookingRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
           return await _bookingRepository.GetAllAsync();
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _bookingRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateBookingAsync(Booking booking)
        {
            if (booking.BookingID <= 0)
            {
                throw new ArgumentException("Booking ID must be greater than zero.", nameof(booking.BookingID));
            }


            var existingBooking = await _bookingRepository.GetByIdAsync(booking.BookingID);

            if(existingBooking == null) throw new KeyNotFoundException("Booking not found.");
            

            bool isRoomAvailable = await _bookingRepository.IsRoomAvailable(booking.RoomID, booking.CheckInDate, booking.CheckOutDate, booking.BookingID);

            if (!isRoomAvailable) throw new InvalidOperationException("Room is not available for the selected dates.");


            existingBooking.RoomID = booking.RoomID;
            existingBooking.CustomerID = booking.CustomerID;
            existingBooking.CheckInDate = booking.CheckInDate;
            existingBooking.CheckOutDate = booking.CheckOutDate;
            existingBooking.NumAdults = booking.NumAdults;
            existingBooking.NumChildren = booking.NumChildren;
            existingBooking.TotalPrice = booking.TotalPrice;
            existingBooking.BookingStatus = booking.BookingStatus;
            existingBooking.BookedByEmployeeID = booking.BookedByEmployeeID;


            Room? room = await _roomRepository.GetByIdAsync(booking.RoomID);
            if (room == null)
            {
                
                throw new KeyNotFoundException("Room not found for price calculation.");
            }



           
           return await _bookingRepository.UpdateAsync(existingBooking);
        }

     
    }
}
