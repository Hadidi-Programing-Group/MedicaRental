using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos.Admin
{
    public static class ClaimRequirement
    {
        public const string AdminPolicy = "AdminPolicy";
        public const string ModeratorPolicy = "ModeratorPolicy";
        public const string ClientPolicy = "ClientPolicy";

        public const string AdminClaim = "admin";
        public const string ModeratorClaim = "moderator";
        public const string ClientClaim = "client";
    }
}
