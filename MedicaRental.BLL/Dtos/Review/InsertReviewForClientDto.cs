using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos.Review
{
    public record InsertReviewForClientDto
    (
        int Rating,
        bool IsDeleted,
        string ClientReview,
        Guid ItemId
    );
}
