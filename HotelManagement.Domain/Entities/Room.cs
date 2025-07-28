
namespace HotelManagement.Domain.Entities
{
    public class Room
    {

        public int RoomID { get; private set; }
        public string RoomNumber { get; private set; }
        public int BedCount { get; private set; }
        public int RoomTypeID { get; private set; }
        public decimal? PricePerNight { get; private set; }
        public int RoomStatusID { get; private set; }
        public RoomType RoomType { get; private set; }

        public RoomStatus RoomStatus { get; private set; }

     //   This constructor is used when creating a new room
        public Room(string roomNumber, int bedCount, int roomTypeID, decimal? pricePerNight,int roomStatusID)
        {
            if (string.IsNullOrWhiteSpace(roomNumber))
            {
                throw new ArgumentException("Room number cannot be null or empty.", nameof(roomNumber));
            }
            if (bedCount <= 0)
            {
                throw new ArgumentException("Bed count must be greater than zero.", nameof(bedCount));
            }
            if (roomTypeID <= 0)
            {
                throw new ArgumentException("Room type ID must be greater than zero.", nameof(roomTypeID));
            }
            if (pricePerNight.HasValue && pricePerNight <= 0)
            {
                throw new ArgumentException("Price per night must be greater than zero.", nameof(pricePerNight));
            }

            if (roomStatusID <= 0)
            {
                throw new ArgumentException("Room status ID must be greater than zero.", nameof(roomStatusID));
            }


            RoomNumber = roomNumber;
            BedCount = bedCount;
            RoomTypeID = roomTypeID;
            PricePerNight = pricePerNight;

            RoomID = 0; // Will be set by DAL/DB later
            RoomStatusID = roomStatusID;

            RoomType = new RoomType(); // Initialize RoomType property
            RoomStatus = new RoomStatus(); // Initialize RoomStatus property
        }

        // *** NEW CONSTRUCTOR FOR DATABASE RETRIEVAL WITH JOINED DATA ***
        public Room(int roomID, string roomNumber, int bedCount, int roomTypeID, decimal? pricePerNight, int roomStatusID, string typeName ,string statusName)
        {
            if (roomID <= 0)
            {
                throw new ArgumentException("Room ID must be greater than zero.", nameof(roomID));
            }
            if (string.IsNullOrWhiteSpace(roomNumber))
            {
                throw new ArgumentException("Room number cannot be null or empty.", nameof(roomNumber));
            }
            if (bedCount <= 0)
            {
                throw new ArgumentException("Bed count must be greater than zero.", nameof(bedCount));
            }
            if (roomTypeID <= 0)
            {
                throw new ArgumentException("Room type ID must be greater than zero.", nameof(roomTypeID));
            }
            if (pricePerNight.HasValue && pricePerNight <= 0)
            {
                throw new ArgumentException("Price per night must be greater than zero.", nameof(pricePerNight));
            }
            if (roomStatusID <= 0)
            {
                throw new ArgumentException("Room status ID must be greater than zero.", nameof(roomStatusID));
            }
            RoomID = roomID;
            RoomNumber = roomNumber;
            BedCount = bedCount;
            RoomTypeID = roomTypeID;
            PricePerNight = pricePerNight;
            RoomStatusID = roomStatusID;
            RoomType = new RoomType { RoomTypeID = roomTypeID, TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName), "Room type name cannot be null.") };
            RoomStatus = new RoomStatus { RoomStatusID = roomStatusID, StatusName = statusName ?? throw new ArgumentNullException(nameof(statusName), "Room status name cannot be null.") };
        }


        // This constructor is used for retrieving from the database
        public Room(int roomID, string roomNumber, int bedCount, int roomTypeID, decimal? pricePerNight, int roomStatusID)
        {
            RoomID = roomID;
            RoomNumber = roomNumber;
            BedCount = bedCount;
            RoomTypeID = roomTypeID;
            PricePerNight = pricePerNight;
            RoomStatusID = roomStatusID;

            // Initialize related entities. Their TypeName/StatusName will be populated in MapToRoom.
            // It's good practice to initialize them with their IDs here if available.
            RoomType = new RoomType { RoomTypeID = roomTypeID };
            RoomStatus = new RoomStatus { RoomStatusID = roomStatusID };
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