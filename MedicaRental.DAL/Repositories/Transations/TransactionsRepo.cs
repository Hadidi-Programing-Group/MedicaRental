using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;


namespace MedicaRental.DAL.Repositories;

public class TransactionsRepo : EntityRepo<Transaction>, ITransactionsRepo
{
    private readonly MedicaRentalDbContext _context;

    public TransactionsRepo(MedicaRentalDbContext context) : base(context)
    {
        _context = context;
    }
}
