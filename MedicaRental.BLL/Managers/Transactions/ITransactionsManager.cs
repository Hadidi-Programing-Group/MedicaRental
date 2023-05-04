using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Transactions;
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
        Task<InsertTransactionStatusDto> UpdateTransaction(UpdateTransactionStatusDto updatetransactionDto);
        Task<InsertTransactionStatusDto> InsertTransaction(TransactionDto addedTransaction);
    }
}
