using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Models
{
    public class Brand
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CountryOfOrigin { get; set; } = string.Empty;
        public byte[]? Image { get; set; }
        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
