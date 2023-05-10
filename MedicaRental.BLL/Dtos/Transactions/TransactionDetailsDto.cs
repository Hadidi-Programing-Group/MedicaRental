using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos;
public record TransactionDetailsDto
(
    Guid Id, string PaymentId, string Date, decimal Amount, TransactionStatus Status, IEnumerable<TransactionItemDto> TransactionItems
);

public record TransactionItemDto
(
    Guid ItemId,
    string EndDate,
    int NumberOfDays,
    string ItemName
);