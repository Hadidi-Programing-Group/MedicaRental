using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Repositories;

public class ReportsRepo : EntityRepo<Report>, IReportsRepo
{
    private MedicaRentalDbContext _context;

    public ReportsRepo(MedicaRentalDbContext context) : base(context)
    {
        _context = context;
    }
}
