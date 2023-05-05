using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos.RentOperation
{
    public record GetRentedItemsDto
      (
          Guid Id,
          string RentDate,
          string ReturnDate,
          decimal Price,
          string UserId,
          string UserName,
          Guid ItemId,
          string ItemName
      );
}
