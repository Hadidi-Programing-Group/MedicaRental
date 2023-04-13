using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Repositories;

public class AdminsRepo : EntityRepo<AppUser>, IAdminsRepo
{
    private MedicaRentalDbContext _context;

    public AdminsRepo(MedicaRentalDbContext context) : base(context)
    {
        _context = context;
    }
}
