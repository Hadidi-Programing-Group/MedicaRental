using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicaRental.DAL.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Range(0, 5)]
        public int Rating { get; set; }

        public bool IsDeleted { get; set; }

        public string ClientReview { get; set; } = string.Empty;
     
        [ForeignKey("User")]
        public string ClientId { get; set; } = string.Empty;
        public Client? Client { get; set; }

        [ForeignKey("Item")]
        public int ItemId { get; set; }
        public Item? Item { get; set; }
    }
}
