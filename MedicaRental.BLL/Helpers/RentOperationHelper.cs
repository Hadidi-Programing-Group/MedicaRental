using MedicaRental.BLL.Dtos;
using MedicaRental.DAL.Models;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MedicaRental.BLL.Helpers
{
    public static class RentOperationHelper
    {
        public const string RentDateDesc = "RentDateDesc";

        public const string RentDateAsc = "RentDateAsc";

        public const string ReturnDateDesc = "ReturnDateDesc";

        public const string ReturnDateAsc = "ReturnDateAsc";

        public static Expression<Func<RentOperation, RentOperationDto>> RentOperationDtoSelector_Owner = ro => new
        (
            ro.Id,
            ro.RentDate,
            ro.ReturnDate,
            ro.Price,
            ro.ClientId,
            ro.Client!.Name,
            ro.ItemId,
            ro.Item!.Name,
            ro.ReviewId,
            ro.Review == null ? 0 : ro.Review.Rating
         );

        public static Expression<Func<RentOperation, RentOperationDto>> RentOperationDtoSelector_Renter = ro => new
        (
            ro.Id,
            ro.RentDate,
            ro.ReturnDate,
            ro.Price,
            ro.SellerId,
            ro.Seller!.Name,
            ro.ItemId,
            ro.Item!.Name,
            ro.ReviewId,
            ro.Review == null ? 0 : ro.Review.Rating
         );



        public static Func<IQueryable<RentOperation>, IIncludableQueryable<RentOperation, object>> RentOperationDtoInclude_Renter = source => source
        .Include(ro => ro.Seller).Include(ro => ro.Item).Include(ro => ro.Review!);

        public static Func<IQueryable<RentOperation>, IIncludableQueryable<RentOperation, object>> RentOperationDtoInclude_Owner = source => source
        .Include(ro => ro.Client).Include(ro => ro.Item).Include(ro => ro.Review!);

        public static Func<IQueryable<RentOperation>, IOrderedQueryable<RentOperation>>? GetOrderByQuery(string? orderBy)
        {
            Func<IQueryable<RentOperation>, IOrderedQueryable<RentOperation>>? orderQuery = orderBy switch
            {
                null => null,
                SharedHelper.HighToLow => new(q => q.OrderByDescending(ro => ro.Price)),
                SharedHelper.LowToHigh => new(q => q.OrderBy(ro => ro.Price)),
                SharedHelper.RateDesc => new(q => q.OrderByDescending(ro => ro.Review == null ? 0 : ro.Review.Rating)),
                SharedHelper.RateAsc => new(q => q.OrderBy(ro => ro.Review == null ? 0 : ro.Review.Rating)),
                RentDateDesc => new(q => q.OrderByDescending(ro => ro.RentDate)),
                RentDateAsc => new(q => q.OrderBy(ro => ro.RentDate)),
                ReturnDateDesc => new(q => q.OrderByDescending(ro => ro.ReturnDate)),
                ReturnDateAsc => new(q => q.OrderBy(ro => ro.ReturnDate)),
                _ => throw new ArgumentException()
            };

            return orderQuery;
        }

        public static Func<IQueryable<RentOperation>, IOrderedQueryable<RentOperation>>? GetOrderByQueryForSearch(string? orderBy, string searchText)
        {
            Func<IQueryable<RentOperation>, IOrderedQueryable<RentOperation>>? orderQuery = orderBy switch
            {
                null => new(q => q.OrderByDescending(i => SharedHelper.LevDistance(i.Item!.Name, searchText))),
                SharedHelper.HighToLow => new(q => q.OrderByDescending(i => SharedHelper.LevDistance(i.Item!.Name, searchText)).ThenByDescending(ro => ro.Price)),
                SharedHelper.LowToHigh => new(q => q.OrderByDescending(i => SharedHelper.LevDistance(i.Item!.Name, searchText)).ThenBy(ro => ro.Price)),
                SharedHelper.RateDesc => new(q => q.OrderByDescending(i => SharedHelper.LevDistance(i.Item!.Name, searchText)).ThenByDescending(ro => ro.Review == null ? 0 : ro.Review.Rating)),
                SharedHelper.RateAsc => new(q => q.OrderByDescending(i => SharedHelper.LevDistance(i.Item!.Name, searchText)).ThenBy(ro => ro.Review == null ? 0 : ro.Review.Rating)),
                RentDateDesc => new(q => q.OrderByDescending(i => SharedHelper.LevDistance(i.Item!.Name, searchText)).ThenByDescending(ro => ro.RentDate)),
                RentDateAsc => new(q => q.OrderByDescending(i => SharedHelper.LevDistance(i.Item!.Name, searchText)).ThenBy(ro => ro.RentDate)),
                ReturnDateDesc => new(q => q.OrderByDescending(i => SharedHelper.LevDistance(i.Item!.Name, searchText)).ThenByDescending(ro => ro.ReturnDate)),
                ReturnDateAsc => new(q => q.OrderByDescending(i => SharedHelper.LevDistance(i.Item!.Name, searchText)).ThenBy(ro => ro.ReturnDate)),
                _ => throw new ArgumentException()
            };

            return orderQuery;
        }
    }
}
