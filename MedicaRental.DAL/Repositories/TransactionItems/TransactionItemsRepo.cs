using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Repositories
{
    public class TransactionItemsRepo : EntityRepo<TransactionItem>, ITransactionItemsRepo
    {
        private readonly MedicaRentalDbContext _context;

        public TransactionItemsRepo(MedicaRentalDbContext context):base(context)
        {
            _context = context;
        }
    }
}
