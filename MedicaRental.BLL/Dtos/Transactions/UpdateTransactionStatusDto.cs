using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos.Transactions
{

    public record UpdateTransactionStatusDto(string PaymentId, TransactionStatus Status);
}
