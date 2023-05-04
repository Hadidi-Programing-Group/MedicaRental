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
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = string.Empty;
        public string PyamentId { get; set; } = string.Empty;
        public AppUser? User { get; set; }
        public decimal Amount { get; set; }
        [EnumDataType(typeof(TransactionStatus))]
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    }
    public enum TransactionStatus
    {
        Success, Pending, Failed
    }
}
