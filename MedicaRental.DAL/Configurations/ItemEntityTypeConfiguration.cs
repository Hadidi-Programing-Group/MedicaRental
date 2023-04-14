using MedicaRental.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Configurations;

public class ItemEntityTypeConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.HasKey(i => i.Id);

        builder.HasIndex(i => i.CategoryId);

        builder.HasIndex(i => i.SubCategoryId);

        builder.HasOne(i => i.SubCategory)
        .WithMany(sc => sc.Items)
        .HasForeignKey(i => i.SubCategoryId)
        .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Category)
        .WithMany(sc => sc.Items)
        .HasForeignKey(i => i.CategoryId)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Property(i => i.Price).HasColumnType("money");

        builder.Property(i => i.Image).HasColumnType("image").IsRequired(true);

        builder.Property(i => i.CurrentRenterId).IsRequired(false);

        builder.Property(i => i.IsDeleted).HasDefaultValue(false);
        builder.HasQueryFilter(i => !i.IsDeleted);
    }
}
