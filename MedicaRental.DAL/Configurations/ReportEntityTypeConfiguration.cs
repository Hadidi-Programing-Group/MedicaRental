using MedicaRental.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Configurations;

public class ReportEntityTypeConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.HasKey(r => r.Id);

        builder.HasOne(i => i.Reportee)
        .WithMany(sc => sc.Reports)
        .HasForeignKey(i => i.ReporteeId)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Property(r => r.IsDeleted).HasDefaultValue(false);
        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}
