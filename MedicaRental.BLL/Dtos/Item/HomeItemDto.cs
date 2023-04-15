using MedicaRental.BLL.Dtos;
using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos
{
    public record HomeItemDto
    (
        string StatusMessage,
        HttpStatusCode StatusCode,
        int Id,
        string Name,
        string Description,
        string Serial,
        string Model,
        string Make,
        string Country,
        int Stock,
        decimal Price,
        string Image,
        bool IsDeleted,
        int CategoryId,
        int SubCategoryId,
        string SellerId,
        string? CurrentRenterId
    ) : StatusDto(StatusMessage, StatusCode);
}
