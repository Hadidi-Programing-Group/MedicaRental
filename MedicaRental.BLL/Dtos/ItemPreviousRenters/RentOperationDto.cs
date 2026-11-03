using MedicaRental.BLL.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos
{
    public record RentOperationDto
    (
        int Id,
        DateTime RentDate,
        DateTime ReturnDate,
        ClientBaseDto Client
    );
}
