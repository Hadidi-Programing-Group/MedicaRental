using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Repositories;

public class ItemsRepo : EntityRepo<Item>, IItemsRepo
{
    private readonly MedicaRentalDbContext _context;

    public ItemsRepo(MedicaRentalDbContext context) : base(context)
    {
        _context = context;
    }

    public bool HasRenters(Item item)
    {
        return item.ItemRenters.Any(r => r.ReturnDate > DateTime.Now);   
    }

    public bool HasRenters(IEnumerable<Item> items)
    {
        return items.Any(i => i.ItemRenters.Any(r => r.ReturnDate > DateTime.Now));
    }
}
