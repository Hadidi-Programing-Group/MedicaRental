using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Repositories.ItemsPreviousRenters
{
    public class ItemsPreviousRentersRepo : EntityRepo<ItemPreviousRenters>, IItemsPreviousRentersRepo
    {
        private readonly MedicaRentalDbContext _context;

        public ItemsPreviousRentersRepo(MedicaRentalDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
