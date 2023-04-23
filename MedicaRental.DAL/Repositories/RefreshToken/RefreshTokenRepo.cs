using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Repositories;

public class RefreshTokenRepo : EntityRepo<RefreshToken>, IRefreshTokenRepo
{
    private readonly MedicaRentalDbContext _context;

    public RefreshTokenRepo(MedicaRentalDbContext context): base(context)
    {
        _context = context;
    }
}
