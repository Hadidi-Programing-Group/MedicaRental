using MedicaRental.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Repositories;

public class MessagesRepo : IMessagesRepo
{
    private MedicaRentalDbContext _context;

    public MessagesRepo(MedicaRentalDbContext context)
    {
        _context = context;
    }
}
