using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos.Admin
{
    public class RoleMangerUserInfoDto
    {
        public string? Id { get; set; } 

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? Role { get; set; }
    }
}
