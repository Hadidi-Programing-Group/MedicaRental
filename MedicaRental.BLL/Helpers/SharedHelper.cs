using MedicaRental.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Helpers
{
    public static class SharedHelper
    {
        public const int SearchMaxDistance = 15;

        public const int Take = 12;

        public const string HighToLow = "PriceDesc";

        public const string LowToHigh = "PriceAsc";

        public const string RateDesc = "RateDesc";

        public const string RateAsc = "RateAsc";

        public static readonly Func<string, string?, bool> Lev = (string name, string? text) => MedicaRentalDbContext.LevDist(name, text, SearchMaxDistance) <= SearchMaxDistance;
    }
}
