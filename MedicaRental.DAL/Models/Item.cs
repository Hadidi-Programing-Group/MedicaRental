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
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Serial { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public string Make { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        public decimal Price { get; set; }

        public byte[]? Image { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [ForeignKey("SubCategory")]
        public int SubCategoryId { get; set; }
        public SubCategory? SubCategory { get; set; }

        [ForeignKey("Seller")]
        public string SellerId { get; set; } = string.Empty;
        public Client? Seller { get; set; }

        [ForeignKey("CurrentRenter")]
        public string? CurrentRenterId { get; set; } = string.Empty;
        public Client? CurrentRenter { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        
        public ICollection<ItemPreviousRenters> ItemRenters { get; set; } = new List<ItemPreviousRenters>();

        [NotMapped]
        public bool InStock { get => Stock == 0; }

        [Range(0, 5)]
        [NotMapped]
        public float Rating { get => 0; }
    }
}
