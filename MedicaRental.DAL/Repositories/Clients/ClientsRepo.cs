using MedicaRental.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Repositories;

public class ClientsRepo : IClientsRepo
{
    private readonly MedicaRentalDbContext _context;

    public ClientsRepo(MedicaRentalDbContext context)
    {
        _context = context;
    }
}
