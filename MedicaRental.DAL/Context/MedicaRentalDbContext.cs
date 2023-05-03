using MedicaRental.DAL.Configurations;
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
        public DbSet<RentOperation> RentOperations => Set<RentOperation>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<ReportAction> ReportActions => Set<ReportAction>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<AdPrice> AdPrices => Set<AdPrice>();

        public MedicaRentalDbContext(DbContextOptions<MedicaRentalDbContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDbFunction(typeof(MedicaRentalDbContext).GetMethod(nameof(LevDist), new[] { typeof(string), typeof(string), typeof(int?)})!)
            .HasName("LevenshteinDistance");

            builder.ApplyConfiguration(new ItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new ClientEntityTypeConfiguration());
            builder.ApplyConfiguration(new CategoryEntityTypeConfiguration());
            builder.ApplyConfiguration(new SubCategoryEntityTypeConfiguration());
            builder.ApplyConfiguration(new ReviewEntityTypeConfiguration());
            builder.ApplyConfiguration(new MessageEntityTypeConfiguration());
            builder.ApplyConfiguration(new ReportEntityTypeConfiguration());
            builder.ApplyConfiguration(new RentOperationEntityTypeConfiguration());
            builder.ApplyConfiguration(new ReportActionEntityTypeConfiguration());
            builder.ApplyConfiguration(new CartItemEntityTypeConfiguration());
        }
        

        public static int LevDist (string s1, string? s2, int? maxDistance) => throw new NotSupportedException();

        public async Task UpdateDailyRatings()
        {
           await Items.ForEachAsync(async i =>
            {
                i.Rating = await CalculateDailyRatingForItem(i.Id);
            });

            await SaveChangesAsync();
        }

        private async Task<int> CalculateDailyRatingForItem(Guid id)
        {
            var averageRating = await  Reviews.Where(r => r.ItemId == id).AverageAsync(r => r.Rating);

            return  (int)averageRating;
        }

        public override int SaveChanges()
        {
            HandleSoftDelete();
            return base.SaveChanges();
        }

        private void HandleSoftDelete()
        {
            var entities = ChangeTracker.Entries()
                                .Where(e => e.State == EntityState.Deleted);
            foreach (var entity in entities)
            {
                if (entity.Entity is ISoftDeletable softDeletable)
                {
                    entity.State = EntityState.Modified;
                    softDeletable.IsDeleted = true;
                }
            }
        }
    }
}
