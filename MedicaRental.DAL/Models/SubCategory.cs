using System.ComponentModel.DataAnnotations.Schema;

namespace MedicaRental.DAL.Models
{
    public class SubCategory
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public byte[]? Icon { get; set; }

        [ForeignKey("Category")]
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
