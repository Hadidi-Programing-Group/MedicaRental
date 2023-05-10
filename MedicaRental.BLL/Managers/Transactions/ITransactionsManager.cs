using MedicaRental.BLL.Dtos;
using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers
{
    public interface ITransactionsManager
    {
        Task<TransactionDto?> GetByPaymentIdAsync(string? id);
        Task<bool> UpdateTransaction(UpdateTransactionStatusDto updatetransactionDto);
        Task<Guid?> InsertTransaction(TransactionDto addedTransaction);
        Task<PageDto<GetAllTransactionsDto>> GetAllTransactionsAsync(string userId, int page);
        Task<TransactionDetailsDto?> GetTransactionDetailsAsync(Guid id, string userId);
    }
}
