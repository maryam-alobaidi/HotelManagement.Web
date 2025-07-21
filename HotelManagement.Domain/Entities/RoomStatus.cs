using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain.Entities
{
    public class RoomStatus
    {

        public int RoomStatusID { get; set; }
        public string StatusName { get; set; }
        public string? Description { get; set; }

        public RoomStatus(string statusName, string? description)
        {

            if(string.IsNullOrWhiteSpace(statusName))
            {
                throw new ArgumentException("Status name cannot be null or empty.", nameof(statusName));
            }


            RoomStatusID = 0; // Default value, will be set by the database


            StatusName = statusName.Trim();
            Description = description.Trim();
        }

        public RoomStatus(int roomStatusID, string statusName, string? description)
        {
            RoomStatusID = roomStatusID;
            StatusName = statusName;
            Description = description;
        }

        public RoomStatus()
        {
        }
    }
}
