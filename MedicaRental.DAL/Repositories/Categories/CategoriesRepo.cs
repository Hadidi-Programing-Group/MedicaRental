using MedicaRental.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Repositories;

public class CategoriesRepo : ICategoriesRepo
{
    private readonly MedicaRentalDbContext _context;

    public CategoriesRepo(MedicaRentalDbContext context)
    {
        _context = context;
    }
}
