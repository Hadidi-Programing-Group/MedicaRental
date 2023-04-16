using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos
{
    public record ReviewBaseDto
    (
        Guid Id,
        int Rating,
        string ClientReview,
        string ClientName
    );
}
