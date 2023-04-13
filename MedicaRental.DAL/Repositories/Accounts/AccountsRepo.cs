using MedicaRental.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Repositories;

public class AccountsRepo : IAccountsRepo
{
    private MedicaRentalDbContext _context;

    public AccountsRepo(MedicaRentalDbContext context)
    {
        _context = context;
    }
}
