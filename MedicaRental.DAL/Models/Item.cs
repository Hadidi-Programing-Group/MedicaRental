using MedicaRental.DAL.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Models
{
    public class Item : ISoftDeletable
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Serial { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        public decimal Price { get; set; }

        public byte[]? Image { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsListed { get; set; } = true;

        public DateTime CreationDate { get; init; } 

        [ForeignKey("Brand")]
        public Guid BrandId { get; set; }
        public Brand? Brand { get; set; }

        [ForeignKey("Category")]
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }

        [ForeignKey("SubCategory")]
        public Guid SubCategoryId { get; set; }
        public SubCategory? SubCategory { get; set; }

        [ForeignKey("Seller")]
        public string SellerId { get; set; } = string.Empty;
        public Client? Seller { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        
        public ICollection<RentOperation> ItemRenters { get; set; } = new List<RentOperation>();

        [NotMapped]
        public bool InStock { get => Stock == 0; }

        [Range(0, 5)]
        [NotMapped]
        public int Rating { get => (int)Reviews.Average(r => r.Rating); }
    }
}
