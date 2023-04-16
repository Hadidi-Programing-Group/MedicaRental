using MedicaRental.BLL.Dtos.Validations;
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
        string Image
    );

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
        [Base64StringImageValidation]
        string Image
    );

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

    //include categ, subcateg, reviews
    public record ListedItemDto
    (
        Guid Id,
        string Name,
        decimal Price,
        int Stock,
        string CategoryName,
        string SubCategoryName,
        int Rating
    );

    public record RentOperationItemDto
    (
        Guid ItemId,
        string ItemName,
        decimal Price,
        DateTime RentDate,
        DateTime ReturnDate,
        Guid RenterId,
        Guid OwnerId
    );
}
