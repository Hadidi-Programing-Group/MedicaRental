using MedicaRental.BLL.Dtos.Validations;

namespace MedicaRental.BLL.Dtos
{
    //include brand, categ, subcateg, reviews.then client.then user
    public record SellerItemDto
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
        IEnumerable<ReviewBaseDto> Reviews,
        [Base64StringImageValidation]
        string Image
    );
}
