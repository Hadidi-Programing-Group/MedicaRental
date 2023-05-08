using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.RentOperation;
using MedicaRental.BLL.Helpers;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.Repositories;
using MedicaRental.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        
        public async Task<Guid?> AddRentOperation(InsertRentOperationDto rentOperationDto)
        {
            var item = await _unitOfWork.Items.FindAsync(i => i.Id == rentOperationDto.ItemId, disableTracking: false);

            if (item == null) return null;
            
            var rentOp = new RentOperation()
            {
                ClientId = rentOperationDto.ClientId,
                ItemId = rentOperationDto.ItemId,
                SellerId = rentOperationDto.SellerId,
                RentDate = rentOperationDto.RentDate,
                ReturnDate = rentOperationDto.ReturnDate,
                Price = rentOperationDto.Price
            };

            var res = await _unitOfWork.RentOperations.AddAsync(rentOp);

            item.Stock--;

            try
            {
                _unitOfWork.Save();
                return rentOp.Id;

            }
            catch
            {
                return null;
            }
        }


        // Return today

        public async Task<PageDto<GetRentedItemsDto>?> GetToBeReturnedItems(int page)
        {
            try
            {
                var data = await _unitOfWork.RentOperations.FindAllAsync(
                    selector: ro => new GetRentedItemsDto(
                        ro.Id,
                        ro.RentDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        ro.ReturnDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        ro.Price,
                        ro.ClientId,
                        $"{ro.Client!.User!.FirstName} {ro.Client!.User.LastName}" ,
                        ro.ItemId,
                        ro.Item!.Name
                    ),
                    predicate: ro => ro.ReturnDate.Date <= DateTime.UtcNow.Date,
                    orderBy:ro => ro.OrderByDescending(ro => ro.ReturnDate.Date == DateTime.UtcNow.Date).ThenByDescending(ro => ro.ReturnDate),
                    include: ro => ro.Include(ro => ro.Client).ThenInclude(ro => ro!.User).Include(ro => ro.Item),
                    skip: page > 1 ? (page - 1) * SharedHelper.Take : null,
                    take: SharedHelper.Take
                );

                var count = await _unitOfWork.RentOperations.GetCountAsync(ro => ro.ReturnDate.Date <= DateTime.UtcNow.Date);

                return new(data, count);
            }
            catch (Exception)
            {
                return null;
            }
        }


        public async Task<StatusDto> AcceptReturnAsync(Guid rentOperationId)
        {
            // Find the rent operation
            var rentOperation = await _unitOfWork.RentOperations.FindAsync(ro => ro.Id == rentOperationId, disableTracking: false);
            if (rentOperation == null)
                return new StatusDto("Rent operation not found.", HttpStatusCode.NotFound);

            // Soft delete the rent operation
            rentOperation.IsDeleted = true;

            // Find the item
            var item = await _unitOfWork.Items.FindAsync(i => i.Id == rentOperation.ItemId, disableTracking: false);
            if (item == null)
                return new StatusDto("Item not found.", HttpStatusCode.NotFound);

            // Increase the item stock
            item.Stock++;

            try
            {
                _unitOfWork.Save();
                return new StatusDto("Return accepted successfully.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new StatusDto($"Return couldn't be accepted.\nCause: {ex.Message}", HttpStatusCode.InternalServerError);
            }
        }
    }
}
