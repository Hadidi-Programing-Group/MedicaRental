using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos.Admin
{
    public record UserRoleUpdateStatusDto(bool IsUpdated, string UpdateMessage, HttpStatusCode StatusCode);

}
