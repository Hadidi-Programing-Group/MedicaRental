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
using MedicaRental.DAL.Repositories;
using MedicaRental.DAL.Context;

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
        .Include(ro => ro.Seller).ThenInclude(s => s!.User).Include(ro => ro.Item).Include(ro => ro.Review!);

        public static Func<IQueryable<RentOperation>, IIncludableQueryable<RentOperation, object>> RentOperationDtoInclude_Owner = source => source
        .Include(ro => ro.Client).ThenInclude(s => s!.User).Include(ro => ro.Item).Include(ro => ro.Review!);

        public static Func<IQueryable<RentOperation>, IOrderedQueryable<RentOperation>>? GetOrderByQuery(string? orderBy, string? searchText)
        {
            if (searchText is null)
            {
                return orderBy switch
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
            }

            return orderBy switch
            {
                null => new(q => q.OrderBy(ro => ro.Item)),
                SharedHelper.HighToLow => new(q => q.OrderBy(ro => MedicaRentalDbContext.LevDist(ro.Item!.Name, searchText, SharedHelper.SearchMaxDistance)).ThenByDescending(ro => ro.Price)),
                SharedHelper.LowToHigh => new(q => q.OrderBy(ro => MedicaRentalDbContext.LevDist(ro.Item!.Name, searchText, SharedHelper.SearchMaxDistance)).ThenBy(ro => ro.Price)),
                SharedHelper.RateDesc => new(q => q.OrderBy(ro => MedicaRentalDbContext.LevDist(ro.Item!.Name, searchText, SharedHelper.SearchMaxDistance)).ThenByDescending(ro => ro.Review == null ? 0 : ro.Review.Rating)),
                SharedHelper.RateAsc => new(q => q.OrderBy(ro => MedicaRentalDbContext.LevDist(ro.Item!.Name, searchText, SharedHelper.SearchMaxDistance)).ThenBy(ro => ro.Review == null ? 0 : ro.Review.Rating)),
                RentDateDesc => new(q => q.OrderBy(ro => MedicaRentalDbContext.LevDist(ro.Item!.Name, searchText, SharedHelper.SearchMaxDistance)).ThenByDescending(ro => ro.RentDate)),
                RentDateAsc => new(q => q.OrderBy(ro => MedicaRentalDbContext.LevDist(ro.Item!.Name, searchText, SharedHelper.SearchMaxDistance)).ThenBy(ro => ro.RentDate)),
                ReturnDateDesc => new(q => q.OrderBy(ro => MedicaRentalDbContext.LevDist(ro.Item!.Name, searchText, SharedHelper.SearchMaxDistance)).ThenByDescending(ro => ro.ReturnDate)),
                ReturnDateAsc => new(q => q.OrderBy(ro => MedicaRentalDbContext.LevDist(ro.Item!.Name, searchText, SharedHelper.SearchMaxDistance)).ThenBy(ro => ro.ReturnDate)),
                _ => throw new ArgumentException()
            };

        }
    }
}
