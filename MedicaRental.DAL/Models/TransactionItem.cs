using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Models
{public class TransactionItem
    {
        public Guid Id { get; set; }

        public int NumberOfDays { get; set; }

        [ForeignKey(nameof(Item))]
        public Guid ItemId { get; set; }
        public Item? Item { get; set; }

        [ForeignKey(nameof(Transaction))]
        public Guid TransactionId { get; set; }
        public Transaction? Transaction { get; set; }
    }
}
