using MedicaRental.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Repositories;

public class SubCategoriesRepo : ISubCategoriesRepo
{
    private readonly MedicaRentalDbContext _context;

    public SubCategoriesRepo(MedicaRentalDbContext context)
    {
        _context = context;
    }
}
