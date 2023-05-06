using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Repositories;

public class CartItemRepo : EntityRepo<CartItem>, ICartItemRepo
{
    public CartItemRepo(MedicaRentalDbContext context) : base(context)
    {
    }
}
