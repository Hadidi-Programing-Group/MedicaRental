using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos
{
    public record StatusDto(string StatusMessage, HttpStatusCode StatusCode);
    public record InsertStatusDto(string StatusMessage, HttpStatusCode StatusCode, Guid Id) : StatusDto(StatusMessage, StatusCode);
}