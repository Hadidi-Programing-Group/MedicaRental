using MedicaRental.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Configurations
{
    internal class ItemPreviousRentersEntityTypeConfiguration : IEntityTypeConfiguration<ItemPreviousRenters>
    {
        public void Configure(EntityTypeBuilder<ItemPreviousRenters> builder)
        {
            builder.HasOne(ir => ir.Client)
               .WithMany(c => c.RentedItems)
               .HasForeignKey(i => i.ClientId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ir => ir.Item)
            .WithMany(i => i.ItemRenters)
            .HasForeignKey(i => i.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Property(i => i.IsDeleted).HasDefaultValue(false);
            builder.HasQueryFilter(i => !i.IsDeleted);
        }
    }
}
