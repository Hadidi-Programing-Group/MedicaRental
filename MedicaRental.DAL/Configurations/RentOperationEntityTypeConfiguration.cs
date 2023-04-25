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
    internal class RentOperationEntityTypeConfiguration : IEntityTypeConfiguration<RentOperation>
    {
        public void Configure(EntityTypeBuilder<RentOperation> builder)
        {
            builder.HasOne(ro => ro.Client)
               .WithMany(c => c.RentOperations)
               .HasForeignKey(i => i.ClientId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ro => ro.Item)
            .WithMany(i => i.ItemRenters)
            .HasForeignKey(i => i.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ro => ro.Review)
            .WithOne(i => i.RentOperation)
            .HasForeignKey<Review>(i => i.RentOperationId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Property(ro => ro.Price).HasColumnType("money");

            builder.Property(ro => ro.ReviewId).IsRequired(false);

            builder.Property(i => i.IsDeleted).HasDefaultValue(false);
            builder.HasQueryFilter(i => !i.IsDeleted);
        }
    }
}
