
namespace HotelManagement.Domain.Entities
{
    public class Room
    {

        public int RoomID { get; private set; }
        public string? RoomNumber { get; private set; }
        public int? BedCount { get; private set; }
        public int? RoomTypeID { get; private set; }
        public decimal? PricePerNight { get; private set; }
        public int? RoomStatusID { get; private set; }

        public RoomType RoomType { get; private set; }

        public RoomStatus RoomStatus { get; private set; }

     //   This constructor is used when creating a new room
        public Room(string? roomNumber, int? bedCount, int? roomTypeID, decimal? pricePerNight)
        {
            RoomNumber = roomNumber;
            BedCount = bedCount;
            RoomTypeID = roomTypeID;
            PricePerNight = pricePerNight;

            RoomID = 0; // Will be set by DAL/DB later
            RoomStatusID = 1; // Assuming: 1 for "Available" (default status for new rooms)

            RoomType = new RoomType(); // Initialize RoomType property
            RoomStatus = new RoomStatus(); // Initialize RoomStatus property
        }

        // This constructor is used when retrieving an existing room from the database

        public Room(int roomID, string? roomNumber, int? bedCount, int? roomTypeID, decimal? pricePerNight, int? roomStatusID)
        {
            RoomID = roomID; // This is the ID from the database
            RoomNumber = roomNumber;
            BedCount = bedCount;
            RoomTypeID = roomTypeID;
            PricePerNight = pricePerNight;
            RoomStatusID = roomStatusID; // Use the status from the DB

            // Initialize related entities (navigation properties) for consistency.
            // Their actual data will be loaded separately or lazily.
            RoomType = new RoomType();
            RoomStatus = new RoomStatus();
        }


        private Room()
        {

        }

        public void UpdatePrice(decimal newPrice)
        {
            if(newPrice <= 0)
            {
                throw new ArgumentException("Price must be greater than zero.");
            }
            PricePerNight = newPrice;
        }

    }
}