using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain.Entities
{
    public class RoomType
    {
        public int RoomTypeID { get; set; }
        public string TypeName { get;  set; }
        public string? Description { get; private set; }
        public decimal BasePrice { get; private set; }
        public int? MaxOccupancy { get; private set; }
        public string? Amenities { get; private set; }

        public RoomType(string typeName, string? description, decimal basePrice, int? maxOccupancy, string? amenities)
        {
           
            if(string.IsNullOrEmpty(TypeName))
            {
                throw new ArgumentException("TypeName cannot be null or empty.");
            }

            if(maxOccupancy.Value<=0 && maxOccupancy.HasValue)
            {
                throw new ArgumentException("MaxOccupancy must be greater than zero.");
            }

            if(basePrice<=0)
            {
                throw new ArgumentException("BasePrice must be greater than zero.");
            }

            TypeName = typeName.Trim();
            Description = string.IsNullOrEmpty(description)?null: description.Trim();
            BasePrice = basePrice;
            MaxOccupancy = maxOccupancy;
            Amenities = string.IsNullOrEmpty(amenities) ? null : amenities.Trim();
        }

        public RoomType(int roomTypeID, string typeName, string? description, decimal basePrice, int? maxOccupancy, string? amenities)
        {
            RoomTypeID = roomTypeID;
            TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
            Description = description;
            BasePrice = basePrice;
            MaxOccupancy = maxOccupancy;
            Amenities = amenities;
        }

        public RoomType()
        {
        }

        public void UpdateBasePrice(decimal newBasePrice)
        {
            if (newBasePrice <= 0)
            {
                throw new ArgumentException("BasePrice must be greater than zero.");
            }
            BasePrice = newBasePrice;
        }
    }


}
