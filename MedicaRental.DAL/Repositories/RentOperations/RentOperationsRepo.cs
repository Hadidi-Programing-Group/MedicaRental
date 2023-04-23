using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MedicaRental.DAL.Repositories;

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
