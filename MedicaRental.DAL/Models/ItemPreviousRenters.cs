using MedicaRental.DAL.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Models
{
    public class ItemPreviousRenters : ISoftDeletable
    {
        public int Id { get; set; }

        public DateTime RentDate { get; set; }

        public DateTime ReturnDate { get; set; }

        [ForeignKey("Client")]
        public string ClientId { get; set; } = string.Empty;
        
        [ForeignKey("Item")]
        public int ItemId { get; set; }
        
        public Client? Client { get; set; }
        
        public Item? Item { get; set; }

        public bool IsDeleted { get; set; }
    }
}
