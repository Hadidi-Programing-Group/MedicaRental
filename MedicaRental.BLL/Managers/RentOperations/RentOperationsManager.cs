using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Helpers;
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

        public async Task<IEnumerable<RentOperationDto>?> GetOnRentItemsAsync(string userId, string? orderBy)
        {
            try
            {
                var orderByQuery = RentOperationHelper.GetOrderByQuery(orderBy);
                return await _unitOfWork.RentOperations.FindAllAsync
                    (
                        orderBy: orderByQuery,
                        selector: RentOperationHelper.RentOperationDtoSelector_Renter,
                        predicate: ro => userId == ro.SellerId && ro.ReturnDate > DateTime.Now,
                        include: RentOperationHelper.RentOperationDtoInclude_Renter
                    );
            }
            catch (Exception) { return null; }
        }
        public async Task<IEnumerable<RentOperationDto>?> GetOnRentItemsHistoryAsync(string userId, string? orderBy)
        {
            try
            {
                var orderByQuery = RentOperationHelper.GetOrderByQuery(orderBy);
                return await _unitOfWork.RentOperations.FindAllAsync
                    (
                        orderBy: orderByQuery,
                        selector: RentOperationHelper.RentOperationDtoSelector_Renter,
                        predicate: ro => userId == ro.SellerId && ro.ReturnDate < DateTime.Now,
                        include: RentOperationHelper.RentOperationDtoInclude_Renter
                    );
            }
            catch (Exception) { return null; }
        }
        public async Task<IEnumerable<RentOperationDto>?> GetRentedItemsAsync(string userId, string? orderBy)
        {
            try
            {
                var orderByQuery = RentOperationHelper.GetOrderByQuery(orderBy);
                return await _unitOfWork.RentOperations.FindAllAsync
                    (
                        orderBy: orderByQuery,
                        selector: RentOperationHelper.RentOperationDtoSelector_Owner,
                        predicate: ro => userId == ro.SellerId && ro.ReturnDate > DateTime.Now,
                        include: RentOperationHelper.RentOperationDtoInclude_Owner
                    );
            }
            catch (Exception) { return null; }
        }
        public async Task<IEnumerable<RentOperationDto>?> GetRentedItemsHistoryAsync(string userId, string? orderBy)
        {
            try
            {
                var orderByQuery = RentOperationHelper.GetOrderByQuery(orderBy);
                return await _unitOfWork.RentOperations.FindAllAsync
                    (
                        orderBy: orderByQuery,
                        selector: RentOperationHelper.RentOperationDtoSelector_Owner,
                        predicate: ro => userId == ro.SellerId && ro.ReturnDate < DateTime.Now,
                        include: RentOperationHelper.RentOperationDtoInclude_Owner
                    );
            }
            catch (Exception) { return null; }
        }
    }
}
