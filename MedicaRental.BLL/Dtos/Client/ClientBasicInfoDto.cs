using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos
{
    public record ClientBasicInfoDto
    (
        string Id,
        string Name,
        int Rating
    );
}
