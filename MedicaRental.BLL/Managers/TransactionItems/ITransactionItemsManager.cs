using MedicaRental.BLL.Dtos.CartItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers
{
    public interface ITransactionItemsManager
    {
        public Task<bool> InsertTransactionItems(IEnumerable<CartItemMinimalDto> items, Guid transactionId);
    }
}
