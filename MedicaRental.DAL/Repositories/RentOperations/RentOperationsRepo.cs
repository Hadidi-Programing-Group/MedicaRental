using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Repositories
{
    public class RentOperationsRepo : EntityRepo<RentOperation>, IRentOperationsRepo
    {
        private readonly MedicaRentalDbContext _context;

        public RentOperationsRepo(MedicaRentalDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
