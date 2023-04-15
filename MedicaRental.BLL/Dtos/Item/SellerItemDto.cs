using System.Net;

namespace MedicaRental.BLL.Dtos
{
    public record SellerItemDto
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
        string? CurrentRenterId,
        CategoryDto Category,
        SubCategoryDto SubCategory,
        ClientBasicInfoDto Seller,
        ClientBasicInfoDto CurrentRenter,
        IEnumerable<ReviewBasicDto> Reviews,
        IEnumerable<RentOperationDto> PreviousRents
    ) : StatusDto(StatusMessage, StatusCode);
}
