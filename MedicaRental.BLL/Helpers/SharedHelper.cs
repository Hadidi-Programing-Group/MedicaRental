using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Helpers
{
    public static class SharedHelper
    {
        public const int Take = 12;

        public const string HighToLow = "PriceDesc";

        public const string LowToHigh = "PriceAsc";

        public const string RateDesc = "RateDesc";

        public const string RateAsc = "RateAsc";

        public static int LevDistance(string s, string t)
        {
            int m = s.Length;
            int n = t.Length;
            int[,] d = new int[m + 1, n + 1];
            for (int i = 0; i <= m; i++)
                d[i, 0] = i;
            for (int j = 0; j <= n; j++)
                d[0, j] = j;
            for (int j = 1; j <= n; j++)
            {
                for (int i = 1; i <= m; i++)
                {
                    if (s[i - 1] == t[j - 1])
                        d[i, j] = d[i - 1, j - 1];
                    else
                        d[i, j] = Math.Min(d[i - 1, j] + 1, Math.Min(d[i, j - 1] + 1, d[i - 1, j - 1] + 1));
                }
            }
            return d[m, n];
        }
    }
}
