using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos
{
    public record StatusDto(string StatusMessage, HttpStatusCode StatusCode);

    public record InsertStatusDto(string StatusMessage, HttpStatusCode StatusCode, int Id):StatusDto(StatusMessage, StatusCode);
    
    public record UpdateStatusDto(string StatusMessage, HttpStatusCode StatusCode, int Id):StatusDto(StatusMessage, StatusCode);
    
    public record DeleteStatusDto(string StatusMessage, HttpStatusCode StatusCode):StatusDto(StatusMessage, StatusCode);
}