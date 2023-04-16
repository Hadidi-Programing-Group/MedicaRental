using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Repositories;

public interface IItemsRepo
{
    public bool HasRenters(Item item);
    public bool HasRenters(IEnumerable<Item> items);
}
