using MedicaRental.BLL.Dtos.Validations;

namespace MedicaRental.BLL.Dtos
{
    //include brand, categ, subcateg, seller.then use, reviews
    public record RenterItemDto
    (
        Guid Id,
        string Name,
        string Description,
        string Serial,
        string Model,
        int Stock,
        int Rating,
        decimal Price,
        BrandBaseDto Brand,
        CategoryBaseDto Category,
        SubCategoryBaseDto SubCategory,
        ClientBaseDto Seller,
        IEnumerable<ReviewBaseDto> Reviews,
        [Base64StringImageValidation]
        string Image,
        bool IsAd
    );
}
