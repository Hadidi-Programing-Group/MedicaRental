using MedicaRental.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Configurations;

public class MessageEntityTypeConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.Id);

        builder.HasIndex(c => new { c.SenderId, c.ReceiverId });

        builder.HasOne(m => m.Sender)
        .WithMany(c => c.SentMessages)
        .HasForeignKey(m => m.SenderId)
        .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Receiver)
        .WithMany(c => c.ReceivedMessages)
        .HasForeignKey(m => m.ReceiverId)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Property(m => m.IsDeleted).HasDefaultValue(false);
        builder.HasQueryFilter(m => !m.IsDeleted);
    }
}
