using MedicaRental.BLL.Dtos.Validations;
using System.Net;

namespace MedicaRental.BLL.Dtos
{
    public record AddItemDto
    (
        string Name,
        string Description,
        string Serial,
        string Model,
        string Make,
        string Country,
        int Stock,
        decimal Price,
        [Base64StringImageValidation]
        string Image,
        int CategoryId,
        int SubCategoryId,
        string SellerId
    );
}
