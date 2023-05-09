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
    public enum TransactionStatus
    {
        Success, Pending, Failed
    }

    public class Transaction
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        [ForeignKey("Client")]
        public string ClientId { get; set; } = string.Empty;
        public Client? Client { get; set; }
        
        public string StripePyamentId { get; set; } = string.Empty;
        
        public decimal Amount { get; set; }
        
        [EnumDataType(typeof(TransactionStatus))]
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    }
}
