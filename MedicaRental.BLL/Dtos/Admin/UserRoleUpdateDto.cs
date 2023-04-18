using MedicaRental.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos.Admin
{
    public record UserRoleUpdateDto(string UserId, UserRoles NewRole);
}
