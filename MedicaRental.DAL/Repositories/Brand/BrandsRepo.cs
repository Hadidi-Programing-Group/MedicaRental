using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Repositories
{
    public class BrandsRepo : EntityRepo<Brand>, IBrandsRepo
    {
        private readonly MedicaRentalDbContext _context;

        public BrandsRepo(MedicaRentalDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
