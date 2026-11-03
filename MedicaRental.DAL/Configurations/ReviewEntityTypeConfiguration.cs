using MedicaRental.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Configurations;

public class ReviewEntityTypeConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(r => r.Id);

        builder.HasOne(r => r.Item)
        .WithMany(i => i.Reviews)
        .HasForeignKey(i => i.ItemId)
        .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.RentOperation)
        .WithOne(i => i.Review)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Property(r => r.IsDeleted).HasDefaultValue(false);
        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}
