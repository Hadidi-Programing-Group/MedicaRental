using System.Net;

namespace MedicaRental.BLL.Dtos
{
    public record UpdateItemDto
    (
        string? Name,
        string? Description,
        string? Serial,
        string? Model,
        string? Make,
        string? Country,
        int? Stock,
        decimal? Price,
        string? Image,
        int? CategoryId,
        int? SubCategoryId,
        string? SellerId
    );
}
