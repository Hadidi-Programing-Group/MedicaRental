using MedicaRental.BLL.Dtos.Validations;

namespace MedicaRental.BLL.Dtos
{
    //include seller, brand, reviews
    public record HomeItemDto
    (
        Guid Id,
        string Name,
        string Model,
        decimal Price,
        int Rating,
        string SellerId,
        string SellerName,
        string BrandName,
        [Base64StringImageValidation]
        string Image,
        bool IsAd = false
    );
}
