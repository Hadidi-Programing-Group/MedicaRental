using MedicaRental.BLL.Dtos;
using MedicaRental.DAL.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using MedicaRental.DAL.UnitOfWork;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;

namespace MedicaRental.BLL.Helpers
{
    public static class ItemHelper
    {
        public const string HighToLow = "PriceDesc";
        
        public const string LowToHigh = "PriceAsc";
        
        public const string RateDesc = "RateDesc";
        
        public const string RateAsc = "RateAsc";
        
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
            Convert.ToBase64String(i.Image!)
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
            new(i.Brand!.Id,i.Brand.Name,i.Brand.CountryOfOrigin),
            new(i.Category!.Id, i.Category.Name),
            new(i.SubCategory!.Id, i.SubCategory.Name),
            i.Reviews.Select( r => new ReviewBaseDto(r.Id, r.Rating, r.ClientReview, r.Client!.Name)),
            Convert.ToBase64String(i.Image!)
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
            Convert.ToBase64String(i.Image!)
         );

        public static Func<IQueryable<Item>, IIncludableQueryable<Item, object>> HomeDtoInclude = source => source
        .Include(i => i.Seller).Include(i => i.Brand).Include(i => i.Reviews);

        public static Func<IQueryable<Item>, IIncludableQueryable<Item, object>> RenterDtoInclude = source => source
        .Include(i => i.Brand).Include(i => i.Category).Include(i => i.SubCategory)
        .Include(i => i.Reviews).Include(i => i.Seller).ThenInclude(s => s!.User!);

        public static Func<IQueryable<Item>, IIncludableQueryable<Item, object>> SellerDtoInclude = source => source
        .Include(i => i.Brand).Include(i => i.Category).Include(i => i.SubCategory)
        .Include(i => i.Reviews).ThenInclude(r => r.Client!).ThenInclude(c => c!.User!);


        public static Func<IQueryable<Item>, IOrderedQueryable<Item>>? GetOrderByQuery(string? orderBy)
        {

            Func<IQueryable<Item>, IOrderedQueryable<Item>>? orderQuery = orderBy switch
            {
                null => null,
                HighToLow => new(q => q.OrderByDescending(i => i.Price)),
                LowToHigh => new(q => q.OrderBy(i => i.Price)),
                RateDesc => new(q => q.OrderByDescending(i => i.Rating)),
                RateAsc => new(q => q.OrderBy(i => i.Rating)),
                _ => throw new ArgumentException()
            };

            return orderQuery;
        }

        public static Func<IQueryable<Item>, IOrderedQueryable<Item>>? GetOrderByQueryForSearch(string? orderBy, string searchText)
        {

            Func<IQueryable<Item>, IOrderedQueryable<Item>>? orderQuery = orderBy switch
            {
                null => new(q => q.OrderByDescending(i => LevDistance(i.Name, searchText))),
                HighToLow => new(q => q.OrderByDescending(i => LevDistance(i.Name, searchText)).ThenByDescending(i => i.Price)),
                LowToHigh => new(q => q.OrderByDescending(i => LevDistance(i.Name, searchText)).ThenBy(i => i.Price)),
                RateDesc => new(q => q.OrderByDescending(i => LevDistance(i.Name, searchText)).ThenByDescending(i => i.Rating)),
                RateAsc => new(q => q.OrderByDescending(i => LevDistance(i.Name, searchText)).ThenBy(i => i.Rating)),
                _ => throw new ArgumentException()
            };

            return orderQuery;
        }

        public static int LevDistance(string s, string t)
        {
            int m = s.Length;
            int n = t.Length;
            int[,] d = new int[m + 1, n + 1];
            for (int i = 0; i <= m; i++)
                d[i, 0] = i;
            for (int j = 0; j <= n; j++)
                d[0, j] = j;
            for (int j = 1; j <= n; j++)
            {
                for (int i = 1; i <= m; i++)
                {
                    if (s[i - 1] == t[j - 1])
                        d[i, j] = d[i - 1, j - 1];
                    else
                        d[i, j] = Math.Min(d[i - 1, j] + 1, Math.Min(d[i, j - 1] + 1, d[i - 1, j - 1] + 1));
                }
            }
            return d[m, n];
        }
    }
}
