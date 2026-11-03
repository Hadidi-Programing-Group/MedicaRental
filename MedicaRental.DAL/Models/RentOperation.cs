using MedicaRental.DAL.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Models
{
    public class RentOperation : ISoftDeletable
    {
        public Guid Id { get; set; }

        public DateTime RentDate { get; set; }

        public DateTime ReturnDate { get; set; }

        public decimal Price { get; set; }

        [ForeignKey("Client")]
        public string ClientId { get; set; } = string.Empty;
        public Client? Client { get; set; }

        [ForeignKey("Seller")]
        public string SellerId { get; set; } = string.Empty;
        public Client? Seller { get; set; }

        [ForeignKey("Item")]
        public Guid ItemId { get; set; }
        public Item? Item { get; set; }

        [ForeignKey("Review")]
        public Guid ReviewId { get; set; }
        public Review? Review { get; set; }

        public bool IsDeleted { get; set; }
    }
}
