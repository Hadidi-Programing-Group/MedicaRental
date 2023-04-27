using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Helpers;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.Repositories;
using MedicaRental.DAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers
{
    public class RentOperationsManager : IRentOperationsManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public RentOperationsManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PageDto<RentOperationDto>?> GetOnRentItemsAsync(string userId, int page, string? orderBy, string? searchText)
        {
            try
            {
                var orderByQuery = RentOperationHelper.GetOrderByQuery(orderBy, searchText);
                var data = await _unitOfWork.RentOperations.FindAllAsync
                    (
                        orderBy: orderByQuery,
                        selector: RentOperationHelper.RentOperationDtoSelector_Renter,
                        predicate: ro => userId == ro.SellerId && ro.ReturnDate > DateTime.UtcNow && (searchText == null || MedicaRentalDbContext.LevDist(ro.Item!.Name, searchText, SharedHelper.SearchMaxDistance) <= SharedHelper.SearchMaxDistance),
                        include: RentOperationHelper.RentOperationDtoInclude_Renter,
                        skip: page > 1 ? SharedHelper.Take * page : null,
                        take: SharedHelper.Take
                    );

                var count = await _unitOfWork.RentOperations.GetCountAsync
                    (
                        predicate: ro => userId == ro.SellerId && ro.ReturnDate > DateTime.UtcNow && (searchText == null ||  MedicaRentalDbContext.LevDist(ro.Item!.Name, searchText, SharedHelper.SearchMaxDistance) <= SharedHelper.SearchMaxDistance)
                    );

                return new(data, count);
            }
            catch (Exception) { return null; }
        }
        public async Task<PageDto<RentOperationDto>?> GetOnRentItemsHistoryAsync(string userId, int page, string? orderBy, string? searchText)
        {
            try
            {

                var orderByQuery = RentOperationHelper.GetOrderByQuery(orderBy, searchText);

                var data = await _unitOfWork.RentOperations.FindAllAsync
                    (
                        orderBy: orderByQuery,
                        selector: RentOperationHelper.RentOperationDtoSelector_Renter,
                        predicate: ro => userId == ro.SellerId && ro.ReturnDate < DateTime.UtcNow && (searchText == null ||  MedicaRentalDbContext.LevDist(ro.Item!.Name, searchText, SharedHelper.SearchMaxDistance) <= SharedHelper.SearchMaxDistance),
                        include: RentOperationHelper.RentOperationDtoInclude_Renter,
                        skip: page > 1 ? SharedHelper.Take * page : null,
                        take: SharedHelper.Take
                    );

                var count = await _unitOfWork.RentOperations.GetCountAsync
                    (
                        predicate: ro => userId == ro.SellerId && ro.ReturnDate < DateTime.UtcNow && (searchText == null ||  MedicaRentalDbContext.LevDist(ro.Item!.Name, searchText, SharedHelper.SearchMaxDistance) <= SharedHelper.SearchMaxDistance)
                    );
                return new(data, count);
            }
            catch (Exception) { return null; }
        }
        public async Task<PageDto<RentOperationDto>?> GetRentedItemsAsync(string userId, int page, string? orderBy, string? searchText)
        {
            try
            {
                var orderByQuery = RentOperationHelper.GetOrderByQuery(orderBy, searchText);
                var data = await _unitOfWork.RentOperations.FindAllAsync
                    (
                        orderBy: orderByQuery,
                        selector: RentOperationHelper.RentOperationDtoSelector_Owner,
                        predicate: ro => userId == ro.SellerId && ro.ReturnDate > DateTime.UtcNow && (searchText == null ||  MedicaRentalDbContext.LevDist(ro.Item!.Name, searchText, SharedHelper.SearchMaxDistance) <= SharedHelper.SearchMaxDistance),
                        include: RentOperationHelper.RentOperationDtoInclude_Owner,
                        skip: page > 1 ? SharedHelper.Take * page : null,
                        take: SharedHelper.Take
                    );

                var count = await _unitOfWork.RentOperations.GetCountAsync
                    (
                        predicate: ro => userId == ro.SellerId && ro.ReturnDate > DateTime.UtcNow && (searchText == null ||  MedicaRentalDbContext.LevDist(ro.Item!.Name, searchText, SharedHelper.SearchMaxDistance) <= SharedHelper.SearchMaxDistance)
                    );

                return new(data, count);
            }
            catch (Exception) { return null; }
        }
        public async Task<PageDto<RentOperationDto>?> GetRentedItemsHistoryAsync(string userId, int page, string? orderBy, string? searchText)
        {
            try
            {
                var orderByQuery = RentOperationHelper.GetOrderByQuery(orderBy, searchText);
                var data = await _unitOfWork.RentOperations.FindAllAsync
                    (
                        orderBy: orderByQuery,
                        selector: RentOperationHelper.RentOperationDtoSelector_Owner,
                        predicate: ro => userId == ro.SellerId && ro.ReturnDate < DateTime.UtcNow && (searchText == null ||  MedicaRentalDbContext.LevDist(ro.Item!.Name, searchText, SharedHelper.SearchMaxDistance) <= SharedHelper.SearchMaxDistance),
                        include: RentOperationHelper.RentOperationDtoInclude_Owner,
                        skip: page > 1 ? SharedHelper.Take * page : null,
                        take: SharedHelper.Take
                    );
                var count = await _unitOfWork.RentOperations.GetCountAsync
                    (
                        predicate: ro => userId == ro.SellerId && ro.ReturnDate < DateTime.UtcNow && (searchText == null ||  MedicaRentalDbContext.LevDist(ro.Item!.Name, searchText, SharedHelper.SearchMaxDistance) <= SharedHelper.SearchMaxDistance)
                    );

                return new(data, count);
            }
            catch (Exception) { return null; }
        }

        public async Task<ItemHasBeenRentedToUserDto> GetRentingStatus(string userId, Guid ItemId)
        {
            var RentOps = await _unitOfWork.RentOperations.FindAllAsync
                (
                   predicate: ro => ro.ClientId == userId && ro.ItemId == ItemId
                );
            if(RentOps is null)
            {
                return new ItemHasBeenRentedToUserDto(isRented:false);
            }

            foreach(var rentOp in RentOps)
            {
                if(rentOp.ReviewId is null)
                    return new ItemHasBeenRentedToUserDto(isRented: true);
            }

            return new ItemHasBeenRentedToUserDto(isRented: false);
        }
    }
}
