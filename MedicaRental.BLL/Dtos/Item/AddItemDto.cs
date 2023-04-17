using MedicaRental.DAL.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace MedicaRental.BLL.Dtos
{
    public record AddItemDto
    (
        string Name,
        string Description,
        string Serial,
        string Model,
        int Stock,
        decimal Price,
        string Image,
        bool IsListed,
        Guid BrandId,
        Guid CategoryId,
        Guid SubCategoryId,
        string SellerId
    );
}
