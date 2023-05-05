using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos
{
    public record ItemMinimalDto
    (
        Guid Id,
        string Name,
        decimal Price
    );
}
