using MedicaRental.BLL.Dtos;
using MedicaRental.DAL.Models;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using MedicaRental.DAL.Context;

namespace MedicaRental.BLL.Helpers
{
    public static class ItemHelper
    {
        public const string DateCreatedDesc = "DateCreatedDesc";
        public const string DateCreatedAsc = "DateCreatedAsc";

        public static Expression<Func<Item, HomeItemDto>> HomeDtoSelector = i => new
        (
            i.Id,
            i.Name,
            i.Model,
            i.Price,
            i.Rating,
            i.SellerId,
            i.Seller!.Name,
            i.Brand!.Name,
            SharedHelper.GetMimeFromBase64(Convert.ToBase64String(i.Image!)),
            i.Ads
         );

        public static Expression<Func<Item, SellerItemDto>> SellerDtoSelector = i => new
        (
            i.Id,
            i.Name,
            i.Description,
            i.Serial,
            i.Model,
            i.Stock,
            i.Rating,
            i.Price,
            new(i.Brand!.Id, i.Brand.Name, i.Brand.CountryOfOrigin),
            new(i.Category!.Id, i.Category.Name),
            new(i.SubCategory!.Id, i.SubCategory.Name),
            i.Reviews.Select(r => new ReviewBaseDto(r.Id, r.Rating, r.ClientReview, r.Client!.Name)),
            SharedHelper.GetMimeFromBase64(Convert.ToBase64String(i.Image!))
         );

        public static Expression<Func<Item, RenterItemDto>> RenterDtoSelector = i => new
        (
            i.Id,
            i.Name,
            i.Description,
            i.Serial,
            i.Model,
            i.Stock,
            i.Rating,
            i.Price,
            new(i.Brand!.Id, i.Brand.Name, i.Brand.CountryOfOrigin),
            new(i.Category!.Id, i.Category.Name),
            new(i.SubCategory!.Id, i.SubCategory.Name),
            new(i.Seller!.Id, i.Seller.Name, i.Seller.Rating),
            i.Reviews.Select(r => new ReviewBaseDto(r.Id, r.Rating, r.ClientReview, r.Client!.Name)),
            SharedHelper.GetMimeFromBase64(Convert.ToBase64String(i.Image!))
         );

        public static Expression<Func<Item, ListItemDto>> ListedDtoSelector = i => new
        (
            i.Id,
            i.Name,
            i.Price,
            i.Stock,
            i.Category!.Name,
            i.SubCategory!.Name,
            i.Rating
         );

        public static Func<IQueryable<Item>, IIncludableQueryable<Item, object>> HomeDtoInclude = source => source
        .Include(i => i.Seller).Include(i => i.Brand).Include(i => i.Reviews);

        public static Func<IQueryable<Item>, IIncludableQueryable<Item, object>> RenterDtoInclude = source => source
        .Include(i => i.Brand).Include(i => i.Category).Include(i => i.SubCategory)
        .Include(i => i.Reviews).ThenInclude(r => r.Client).ThenInclude(c => c!.User)
        .Include(i => i.Seller).ThenInclude(s => s!.User!);

        public static Func<IQueryable<Item>, IIncludableQueryable<Item, object>> SellerDtoInclude = source => source
        .Include(i => i.Brand).Include(i => i.Category).Include(i => i.SubCategory)
        .Include(i => i.Reviews).ThenInclude(r => r.Client!).ThenInclude(c => c!.User!);

        public static Func<IQueryable<Item>, IIncludableQueryable<Item, object>> ListedDtoInclude = source => source
        .Include(i => i.Category).Include(i => i.SubCategory).Include(i => i.Reviews);


        public static Func<IQueryable<Item>, IOrderedQueryable<Item>>? GetOrderByQuery(string? orderBy, string? searchText)
        {
            if (searchText is null)
                return orderBy switch
                {
                    null => new(q => q.OrderByDescending(i => i.Ads == true)),
                    SharedHelper.HighToLow => new(q => q.OrderByDescending(i => i.Ads==true).ThenByDescending(i => i.Price)),
                    SharedHelper.LowToHigh => new(q => q.OrderByDescending(i => i.Ads == true).ThenBy(i => i.Price)),
                    SharedHelper.RateDesc => new(q => q.OrderByDescending(i => i.Ads == true).ThenByDescending(i => i.Rating)),
                    SharedHelper.RateAsc => new(q => q.OrderByDescending(i => i.Ads == true).ThenBy(i => i.Rating)),
                    DateCreatedDesc => new(q => q.OrderByDescending(i => i.Ads == true).ThenByDescending(i => i.CreationDate)),
                    DateCreatedAsc => new(q => q.OrderByDescending(i => i.Ads == true).ThenBy(i => i.CreationDate)),
                    _ => throw new ArgumentException()
                };

            return orderBy switch
            {
                null => new(q => q.OrderBy(i => MedicaRentalDbContext.LevDist(i.Name, searchText, SharedHelper.SearchMaxDistance)).ThenByDescending(i => i.Ads == true)),
                SharedHelper.HighToLow => new(q => q.OrderBy(i => MedicaRentalDbContext.LevDist(i.Name, searchText, SharedHelper.SearchMaxDistance)).ThenByDescending(i=>i.Ads==true).ThenByDescending(i => i.Price)),
                SharedHelper.LowToHigh => new(q => q.OrderBy(i => MedicaRentalDbContext.LevDist(i.Name, searchText, SharedHelper.SearchMaxDistance)).ThenByDescending(i => i.Ads==true).ThenBy(i => i.Price)),
                SharedHelper.RateDesc => new(q => q.OrderBy(i => MedicaRentalDbContext.LevDist(i.Name, searchText, SharedHelper.SearchMaxDistance)).ThenByDescending(i => i.Ads == true).ThenByDescending(i => i.Rating)),
                SharedHelper.RateAsc => new(q => q.OrderBy(i => MedicaRentalDbContext.LevDist(i.Name, searchText, SharedHelper.SearchMaxDistance)).ThenByDescending(i => i.Ads == true).ThenBy(i => i.Rating)),
                DateCreatedDesc => new(q => q.OrderBy(i => MedicaRentalDbContext.LevDist(i.Name, searchText, SharedHelper.SearchMaxDistance)).ThenByDescending(i => i.Ads == true).ThenByDescending(i => i.CreationDate)),
                DateCreatedAsc => new(q => q.OrderBy(i => MedicaRentalDbContext.LevDist(i.Name, searchText, SharedHelper.SearchMaxDistance)).ThenByDescending(i => i.Ads == true).ThenBy(i => i.CreationDate)),
                _ => throw new ArgumentException()
            };
        }

        public static Item MapAddDto(AddItemDto item)
        {
            return new()
            {
                Name = item.Name,
                Description = item.Description,
                Serial = item.Serial,
                Model = item.Model,
                Stock = item.Stock,
                Price = item.Price,
                Image = Convert.FromBase64String(item.Image),
                IsListed = item.IsListed,
                BrandId = item.BrandId,
                CategoryId = item.CategoryId,
                SubCategoryId = item.SubCategoryId,
                SellerId = item.SellerId
            };
        }

        public static Item MapUpdateDto(Item item, UpdateItemDto updated)
        {
            item.Name = updated.Name is null ? item.Name : updated.Name;
            item.Description = updated.Description is null ? item.Description : updated.Description;
            item.Serial = updated.Serial is null ? item.Serial : updated.Serial;
            item.Model = updated.Model is null ? item.Model : updated.Model;
            item.Stock = updated.Stock is null ? item.Stock : (int)updated.Stock;
            item.Price = updated.Price is null ? item.Price : (decimal)updated.Price;
            item.Image = updated.Image is null ? item.Image : Convert.FromBase64String(updated.Image);
            item.IsListed = updated.IsListed is null ? item.IsListed : (bool)updated.IsListed;
            item.BrandId = updated.BrandId is null ? item.BrandId : (Guid)updated.BrandId;
            item.CategoryId = updated.CategoryId is null ? item.CategoryId : (Guid)updated.CategoryId;
            item.SubCategoryId = updated.SubCategoryId is null ? item.SubCategoryId : (Guid)updated.SubCategoryId;

            return item;
        }
    }
}
