using HotelManagement.Domain.Enums;

namespace HotelManagement.Domain.Entities
{
    
    public class Booking
    {
      
        public int BookingID { get; set; }
        public int RoomID { get; set; }
        public int CustomerID { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public DateTime BookingDate { get; set; }
        public int NumAdults { get; set; }
        public int? NumChildren { get; set; }
        public decimal TotalPrice { get; set; }
        public  BookingStatusEnum BookingStatus { get; set; }// حالة الحجز (معلقة، مؤكدة، ملغاة، مكتملة) تعديل في الدتا بيس ؟

        public int? BookedByEmployeeID { get; set; }
        public Employee employee { get; set; }
        public Room Room { get; set; } // خاصية Navigation لكيان الغرفة
        public Customer Customer { get; set; } // خاصية Navigation لكيان العميل

        // Constructor for creating a new booking
        public Booking(int roomID, int customerID, DateTime checkInDate, DateTime checkOutDate,
                 int numAdults, int? numChildren, int? bookedByEmployeeID)
        {
            // Validation for core invariants
            if (roomID <= 0) throw new ArgumentException("Room ID must be positive.", nameof(roomID));
            if (customerID <= 0) throw new ArgumentException("Customer ID must be positive.", nameof(customerID));
            if (bookedByEmployeeID.HasValue && bookedByEmployeeID.Value <= 0)
            {
                // هذا الشرط يعني: إذا كان هناك قيمة و كانت هذه القيمة أقل أو تساوي صفر
                throw new ArgumentException("Employee ID must be positive if provided.", nameof(bookedByEmployeeID));
            }
            if (checkInDate.Date >= checkOutDate.Date) throw new ArgumentException("Check-out date must be after check-in date.", nameof(checkOutDate)); // Use .Date to compare only dates
            if (numAdults <= 0 && numChildren <= 0) throw new ArgumentException("At least one adult or child is required for the booking.", nameof(numAdults));

            // Assign properties (use .Date for dates if time component is irrelevant for logic)
            RoomID = roomID;
            CustomerID = customerID;
            CheckInDate = checkInDate.Date;
            CheckOutDate = checkOutDate.Date;
            BookingDate = DateTime.UtcNow; // Prefer UTC for consistency
            NumAdults = numAdults;
            NumChildren = numChildren;
            BookedByEmployeeID = bookedByEmployeeID;

            TotalPrice = 0; // Initial price is 0, will be calculated later
            BookingStatus = BookingStatusEnum.Pending; // Default status
        }
        //for retrieving booking details
        public Booking(int bookingID, int roomID, int customerID, DateTime checkInDate, DateTime checkOutDate, DateTime bookingDate, int numAdults, int? numChildren, decimal totalPrice, BookingStatusEnum bookingStatus, int? bookedByEmployeeID)
        {
            BookingID = bookingID;
            RoomID = roomID;
            CustomerID = customerID;
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
            BookingDate = bookingDate;
            NumAdults = numAdults;
            NumChildren = numChildren;
            TotalPrice = totalPrice;
            BookingStatus = bookingStatus;
            BookedByEmployeeID = bookedByEmployeeID;
          
        }

        private Booking()
        {

        }

     

        public void ConfirmBooking()
        {
            if (BookingStatus == BookingStatusEnum.Pending)
            {
                BookingStatus = BookingStatusEnum.Confirmed;
                // Add other side effects, e.g., trigger an email, update room status, etc.
            }
            else
            {
                throw new InvalidOperationException($"Cannot confirm booking. Current status is {BookingStatus}.");
            }
        }
        public void CancelBooking()
        {
            if (BookingStatus == BookingStatusEnum.Completed)
            {
                throw new InvalidOperationException("Cannot cancel a completed booking.");
            }

            BookingStatus = BookingStatusEnum.Cancelled;
            // Add other side effects, e.g., free up the room, calculate refund, etc.

        }


        public decimal CalculateTotalPrice(decimal pricePerNight)
        {
            if (pricePerNight <= 0)
            {
                throw new ArgumentException("Price per night must be greater than zero.", nameof(pricePerNight));
            }

            if (CheckOutDate <= CheckInDate)
            {
                throw new InvalidOperationException("Check-out date must be after check-in date.");
            }

            int numNights = (CheckOutDate - CheckInDate).Days;

            return TotalPrice = numNights * pricePerNight;
        }
    }
}
