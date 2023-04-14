using MedicaRental.DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MedicaRental.DAL.Context
{
    public class MedicaRentalDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<Item> Items => Set<Item>();
        public DbSet<Client> Clients => Set<Client>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<SubCategory> SubCategories => Set<SubCategory>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<Report> Reports => Set<Report>();
        public DbSet<ItemPreviousRenters> ItemPreviousRenters => Set<ItemPreviousRenters>();

        public MedicaRentalDbContext(DbContextOptions<MedicaRentalDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Item>(entity =>
            {
                entity.HasKey(i => i.Id);

                entity.HasIndex(i => i.CategoryId);

                entity.HasIndex(i => i.SubCategoryId);

                entity.HasOne(i => i.SubCategory)
                .WithMany(sc => sc.Items)
                .HasForeignKey(i => i.SubCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(i => i.Category)
                .WithMany(sc => sc.Items)
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.Property(i => i.Price).HasColumnType("money");

                entity.Property(i => i.Image).HasColumnType("image").IsRequired(true);
                
                entity.Property(i => i.CurrentRenterId).IsRequired(false);

                entity.Property(i => i.IsDeleted).HasDefaultValue(false);
                entity.HasQueryFilter(i => !i.IsDeleted);
            });

            builder.Entity<Client>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.HasIndex(c => c.Ssn).IsUnique();
                entity.Property(c => c.NationalIdImage).HasColumnType("image").IsRequired(true);
                entity.Property(c => c.UnionCardImage).HasColumnType("image").IsRequired(true);
            });

            builder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Icon).HasColumnType("image").IsRequired(true);
            });

            builder.Entity<SubCategory>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Icon).HasColumnType("image").IsRequired(true);
            });

            builder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.HasOne(r => r.Item)
                .WithMany(i => i.Reviews)
                .HasForeignKey(i => i.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.Property(r => r.IsDeleted).HasDefaultValue(false);
                entity.HasQueryFilter(r => !r.IsDeleted);
            });

            builder.Entity<Message>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.HasOne(m => m.Sender)
                .WithMany(c => c.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Receiver)
                .WithMany(c => c.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.Property(m => m.IsDeleted).HasDefaultValue(false);
                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            builder.Entity<Report>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.HasOne(i => i.Reportee)
                .WithMany(sc => sc.Reports)
                .HasForeignKey(i => i.ReporteeId)
                .OnDelete(DeleteBehavior.Restrict);

                //entity.HasOne(i => i.Reported)
                //.WithMany(sc => sc.Reports)
                //.HasForeignKey(i => i.ReporteeId)
                //.OnDelete(DeleteBehavior.Restrict);

                entity.Property(r => r.IsDeleted).HasDefaultValue(false);
                entity.HasQueryFilter(r => !r.IsDeleted);
            });

            builder.Entity<ItemPreviousRenters>(entity =>
            {
                entity.HasOne(ir => ir.Client)
                .WithMany(c => c.RentedItems)
                .HasForeignKey(i => i.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ir => ir.Item)
                .WithMany(i => i.ItemRenters)
                .HasForeignKey(i => i.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.Property(i => i.IsDeleted).HasDefaultValue(false);
                entity.HasQueryFilter(i => !i.IsDeleted);
            });
        }

        public override void RemoveRange(IEnumerable<object> entities)
        {
            foreach (var entity in entities)
            {
                if (entity is ISoftDeletable deletable)
                {
                    deletable.IsDeleted = true;
                    Entry(deletable).State = EntityState.Modified;
                }
                else
                {
                    Entry(entity).State = EntityState.Deleted;
                }
            }
        }

        public override void RemoveRange(params object[] entities)
        {
            foreach (var entity in entities)
            {
                if (entity is ISoftDeletable deletable)
                {
                    deletable.IsDeleted = true;
                    Entry(deletable).State = EntityState.Modified;
                }
                else
                {
                    Entry(entity).State = EntityState.Deleted;
                }
            }
        }
        
        public override EntityEntry Remove(object entity)
        {
            if (entity is ISoftDeletable deletable)
            {
                deletable.IsDeleted = true;
                Entry(deletable).State = EntityState.Modified;

                return Entry(entity);
            }
            else
            {
                return base.Remove(entity);
            }
        }

        public override EntityEntry<TEntity> Remove<TEntity>(TEntity entity)
        {
            if (entity is ISoftDeletable deletable)
            {
                deletable.IsDeleted = true;
                Entry(deletable).State = EntityState.Modified;

                return Entry(entity);
            }
            else
            {
                return base.Remove(entity);
            }
        }
    }
}
