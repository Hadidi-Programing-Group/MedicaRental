using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos
{
    public class TransactionDto
    {
        public string ClientId { get; set; } = string.Empty;
        public string PaymentId { get; set; } = string.Empty;
        public decimal Ammount { get; set; }
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    }
}
