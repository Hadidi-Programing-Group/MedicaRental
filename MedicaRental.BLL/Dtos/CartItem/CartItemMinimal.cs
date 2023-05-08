using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos.CartItem
{
    public record CartItemMinimalDto
    (
        Guid Id,
        Guid ItemId,
        int NumberOfDays
    );
}
