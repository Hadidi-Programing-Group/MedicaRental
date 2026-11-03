using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Repositories;

public class MessagesRepo : EntityRepo<Message>, IMessagesRepo
{
    private MedicaRentalDbContext _context;

    public MessagesRepo(MedicaRentalDbContext context) : base(context)
    {
        _context = context;
    }
}
