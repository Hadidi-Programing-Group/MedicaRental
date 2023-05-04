using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Transactions;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.UnitOfWork;

namespace MedicaRental.BLL.Managers
{

    public class TransactionsManager : ITransactionsManager
    {
        private readonly IUnitOfWork _unitOfWork;
        public TransactionsManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TransactionDto?> GetByPaymentIdAsync(string? id)
        {
            Transaction? t = await _unitOfWork.Trasactions.FindAsync(t => t.PyamentId == id);
            if (t is null)
            {
                return null;
            }
            return new TransactionDto()
            {
                Ammount = t.Amount,
                PaymentId = t.PyamentId,
                UserId = t.UserId,
                Status = t.Status
            };
        }
        public async Task<InsertTransactionStatusDto> UpdateTransaction(UpdateTransactionStatusDto updatetransactionDto)
        {
            Transaction? t = await _unitOfWork.Trasactions.FindAsync(t => t.PyamentId == updatetransactionDto.PaymentId, disableTracking: false);
            if (t is null)
            {
                return new InsertTransactionStatusDto(
                  isCreated: false,
                  Id: null,
                  StatusMessage: "Can't to find Transaction!");
            }

            t.Status = updatetransactionDto.Status;

            _unitOfWork.Trasactions.Update(t);

            try
            {
                _unitOfWork.Save();
                return new InsertTransactionStatusDto(
                    isCreated: true,
                    Id: t.Id,
                    StatusMessage: "Transaction has been Updated successfully.");
            }

            catch
            {
                return new InsertTransactionStatusDto(
                    isCreated: false,
                    Id: null,
                    StatusMessage: "Failed to update Transaction!");
            }
        }
        public async Task<InsertTransactionStatusDto> InsertTransaction(TransactionDto addedTransaction)
        {


            Transaction transaction = new()
            {
                UserId = addedTransaction.UserId,
                PyamentId = addedTransaction.PaymentId,
                Amount = addedTransaction.Ammount
            };

            await _unitOfWork.Trasactions.AddAsync(transaction);
            try
            {
                _unitOfWork.Save();
                await Console.Out.WriteLineAsync("Tranasction Saved");
                return new InsertTransactionStatusDto(
                    isCreated: true,
                    Id: transaction.Id,
                    StatusMessage: "Transaction has been created successfully.");
            }

            catch
            {
                return new InsertTransactionStatusDto(
                    isCreated: false,
                    Id: null,
                    StatusMessage: "Failed to insert Transaction!");
            }


        }
    }
}
