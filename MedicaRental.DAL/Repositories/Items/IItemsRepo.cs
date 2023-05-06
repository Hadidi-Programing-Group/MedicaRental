using MedicaRental.DAL.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace MedicaRental.DAL.Repositories;

public interface IItemsRepo
{
    public bool HasRenters(Item item);
    public bool HasRenters(IEnumerable<Item> items);
    public decimal ItemsTotalPrice(IEnumerable<Guid> itemIds);
}
