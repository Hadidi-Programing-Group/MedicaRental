using MedicaRental.BLL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers
{
    public interface IRentOperationsManager
    {
        public Task<IEnumerable<RentOperationDto>?> GetRentedItemsAsync(string userId, string? orderBy);
        public Task<IEnumerable<RentOperationDto>?> GetRentedItemsHistoryAsync(string userId, string? orderBy);
        public Task<IEnumerable<RentOperationDto>?> GetOnRentItemsAsync(string userId, string? orderBy);
        public Task<IEnumerable<RentOperationDto>?> GetOnRentItemsHistoryAsync(string userId, string? orderBy);
    }
}
