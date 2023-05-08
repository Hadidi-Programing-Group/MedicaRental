using MedicaRental.BLL.Dtos.CartItem;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers
{
    public class TransactionItemsManager : ITransactionItemsManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionItemsManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> InsertTransactionItems(IEnumerable<CartItemMinimalDto> items, Guid transactionId)
        {

            var transItems = items.Select(i => new TransactionItem
            {
                ItemId = i.ItemId,
                TransactionId = transactionId,
                NumberOfDays = i.NumberOfDays,
            });

            var res = await _unitOfWork.TrasactionItems.AddRangeAsync(transItems);

            try
            {
                _unitOfWork.Save();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
