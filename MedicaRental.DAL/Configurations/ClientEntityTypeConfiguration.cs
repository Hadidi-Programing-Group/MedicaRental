using MedicaRental.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Configurations;

public class ClientEntityTypeConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(c => c.Id);
        builder.HasIndex(c => c.Ssn).IsUnique();
        builder.Property(c => c.NationalIdImage).HasColumnType("image").IsRequired(true);
        builder.Property(c => c.UnionCardImage).HasColumnType("image").IsRequired(true);
    }
}
