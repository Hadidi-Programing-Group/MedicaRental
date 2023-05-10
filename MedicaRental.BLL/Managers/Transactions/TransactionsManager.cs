using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Helpers;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;

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

        public async Task<PageDto<GetAllTransactionsDto>> GetAllTransactionsAsync(string userId, int page)
        {
            var transactions = await _unitOfWork.Trasactions.FindAllAsync(
                predicate: t => t.ClientId == userId,
                skip: (page - 1) * SharedHelper.Take,
                take: SharedHelper.Take,
                selector: t => new GetAllTransactionsDto(t.Id, t.StripePyamentId, t.Date, t.Amount, t.Status)
                );

            var count = await _unitOfWork.Trasactions.GetCountAsync(predicate: t => t.ClientId == userId);

            return new(transactions, count);
        }

        public async Task<TransactionDetailsDto?> GetTransactionDetailsAsync(Guid id, string userId)
        {
            var transaction = await _unitOfWork.Trasactions.FindAsync(
                predicate: t => t.Id == id && t.ClientId == userId,
                include: source => source.Include(t => t.TransactionItems).ThenInclude(ti => ti.Item),
                selector: t => new TransactionDetailsDto(
                    t.Id, 
                    t.StripePyamentId, 
                    t.Date, 
                    t.Amount, 
                    t.Status,
                    t.TransactionItems.Select(ti => new TransactionItemDto(ti.ItemId, t.Date.AddDays(ti.NumberOfDays), ti.NumberOfDays, ti.Item.Name))
                ));

            return transaction;
        }
    }
}
