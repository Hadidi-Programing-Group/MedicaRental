using MedicaRental.BLL.Dtos;
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
            Transaction? t = await _unitOfWork.Trasactions.FindAsync(t => t.StripePyamentId == id);
            if (t is null)
            {
                return null;
            }
            return new TransactionDto()
            {
                Ammount = t.Amount,
                PaymentId = t.StripePyamentId,
                ClientId = t.ClientId,
                Status = t.Status
            };
        }
        public async Task<bool> UpdateTransaction(UpdateTransactionStatusDto updatetransactionDto)
        {
            Transaction? t = await _unitOfWork.Trasactions.FindAsync(t => t.StripePyamentId == updatetransactionDto.PaymentId, disableTracking: false);
            if (t is null)
            {
                return false;
            }

            t.Status = updatetransactionDto.Status;

            _unitOfWork.Trasactions.Update(t);

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
        public async Task<Guid?> InsertTransaction(TransactionDto addedTransaction)
        {
            Transaction transaction = new()
            {
                ClientId = addedTransaction.ClientId,
                StripePyamentId = addedTransaction.PaymentId,
                Amount = addedTransaction.Ammount
            };

            await _unitOfWork.Trasactions.AddAsync(transaction);
            try
            {
                _unitOfWork.Save();
                await Console.Out.WriteLineAsync("Tranasction Saved");
                return transaction.Id;
            }

            catch
            {
                return null;
            }
        }
    }
}
