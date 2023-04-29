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
    internal class ReportActionEntityTypeConfiguration : IEntityTypeConfiguration<ReportAction>
    {
        public void Configure(EntityTypeBuilder<ReportAction> builder)
        {
            builder.HasOne<AppUser>(ra => ra.AppUser)
                  .WithMany()
                  .HasForeignKey(ra => ra.AdminId)
                  .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
