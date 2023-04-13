using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Repositories;

public class ClientsRepo : EntityRepo<Client>, IClientsRepo
{
    private readonly MedicaRentalDbContext _context;

    public ClientsRepo(MedicaRentalDbContext context) : base(context)
    {
        _context = context;
    }

    public void Add(Client newClient)
    {
        _context.Set<Client>().Add(newClient);
    }
}
