using MedicaRental.DAL.Context;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicaRental.DAL.Models
{
    public class Review : ISoftDeletable
    {
        public Guid Id { get; set; }

        [Range(0, 5)]
        public int Rating { get; set; }

        public bool IsDeleted { get; set; }

        public string ClientReview { get; set; } = string.Empty;
     
        [ForeignKey("Client")]
        public string ClientId { get; set; } = string.Empty;
        public Client? Client { get; set; }

        [ForeignKey("Seller")]
        public string SellerId { get; set; } = string.Empty;
        public Client? Seller { get; set; }

        [ForeignKey("Item")]
        public Guid ItemId { get; set; }
        public Item? Item { get; set; }

        [ForeignKey("RentOperation")]
        public Guid RentOperationId { get; set; }
        public RentOperation? RentOperation { get; set; } 
    }
}
