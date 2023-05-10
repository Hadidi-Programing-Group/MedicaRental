using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos;

public record TransactionDetailsDto
(
    Guid Id, string PaymentId, DateTime Date, decimal Amount, TransactionStatus Status, IEnumerable<TransactionItemDto> TransactionItems
);

public record TransactionItemDto
(
    Guid ItemId,
    DateTime EndDate,
    int NumberOfDays,
    string ItemName
);