using MedicaRental.BLL.Dtos.Client;
using MedicaRental.BLL.Dtos.Review;
using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos.Item
{
    /*
 in Home for both owner and renter I'll return Item + Seller
 in Item view for Renter I'll return Categ SubCateg Seller Reviews
 in Item view for Owner I'll return all of them
 */
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
    ) : BaseDto(StatusMessage, StatusCode);

    public record RenterItemDto
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
        IEnumerable<ReviewBasicDto> Reviews
    ) : BaseDto(StatusMessage, StatusCode);
    public record SellerItemDto ();
    public record AddItemDto ();
    public record UpdateItemDto ();
    public record DeleteItemDto ();
}
