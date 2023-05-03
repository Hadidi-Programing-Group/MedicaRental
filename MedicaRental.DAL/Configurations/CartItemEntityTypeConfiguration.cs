using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Configurations
{
    internal class CartItemEntityTypeConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.HasOne<Client>(ca => ca.Client)
                  .WithMany(c => c.CartItems)
                  .HasForeignKey(ca => ca.ClientId)
                  .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
