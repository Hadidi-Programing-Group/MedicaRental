using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos
{
    public record MessageDto
    (
        Guid Id,
        string Message,
        string SenderId,
        string MessageDate,
        MessageStatus MessageStatus
    );
}
