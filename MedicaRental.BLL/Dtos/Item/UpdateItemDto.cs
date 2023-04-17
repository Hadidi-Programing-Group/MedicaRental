using MedicaRental.BLL.Dtos.Validations;

namespace MedicaRental.BLL.Dtos
{
    public record UpdateItemDto
    (
        Guid Id,
        string? Name,
        string? Description,
        string? Serial,
        string? Model,
        int? Stock,
        decimal? Price,
        [Base64StringImageValidation]
        string? Image,
        bool? IsListed,
        Guid? BrandId,
        Guid? CategoryId,
        Guid? SubCategoryId
    );
}
