using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Repositories;

public class SubCategoriesRepo : EntityRepo<SubCategory>, ISubCategoriesRepo
{
    private readonly MedicaRentalDbContext _context;

    public SubCategoriesRepo(MedicaRentalDbContext context) : base(context)
    {
        _context = context;
    }
}
