using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos.Authentication
{
    public class RevokeRefreshTokenDto
    {
        // Nullable as we can recieve it from cookies.
        public string? Token { get; set; }
    }
}
