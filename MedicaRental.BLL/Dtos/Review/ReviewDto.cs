using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos;

    public record ReviewDto(Guid Id, int Rating, bool IsDeleted, string? ClientReview, string? ClientId, string? SellerId, Guid ItemId);

