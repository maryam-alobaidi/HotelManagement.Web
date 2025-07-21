using HotelManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Infrastructure.DAL.Interfaces
{
    public interface IRoomTypesRepository
    {

        Task<int?>  AddAsync(RoomType roomType);
        Task<bool> DeleteAsync(int roomTypeID);
        Task<bool> UpdateAsync(RoomType roomType);
        Task<IEnumerable<RoomType>> GetAllAsync();
        Task<RoomType?> GetByIdAsync(int roomTypeID);

    }
}
