using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Repositories;

public class AdPriceRepo : EntityRepo<AdPrice>, IAdPriceRepo
{
    public AdPriceRepo(MedicaRentalDbContext context) : base(context)
    {
    }
}
