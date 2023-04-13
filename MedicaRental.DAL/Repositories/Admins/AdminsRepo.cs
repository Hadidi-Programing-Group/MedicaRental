using MedicaRental.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Repositories;

public class AdminsRepo : IAdminsRepo
{
    private MedicaRentalDbContext _context;

    public AdminsRepo(MedicaRentalDbContext context)
    {
        _context = context;
    }
}
