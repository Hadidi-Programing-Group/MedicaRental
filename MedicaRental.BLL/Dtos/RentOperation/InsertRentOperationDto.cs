using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos
{
    public record InsertRentOperationDto
    (
        DateTime RentDate,
        DateTime ReturnDate,
        decimal Price,
        string ClientId,
        string SellerId,
        Guid ItemId
    );
}
