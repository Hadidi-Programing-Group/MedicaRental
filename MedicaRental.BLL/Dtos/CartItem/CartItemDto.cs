using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos;

public record CartItemDto(
    Guid Id,
    Guid ItemId,
    string Name,
    string Model,
    decimal PricePerDay,
    string Image,
    int NumberOfDays
);
