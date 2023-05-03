using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Models;

public class AdPrice
{
    public Guid Id { get; set; }

    [Column(TypeName = "money")]
    public decimal Price { get; set; }

    public string Description { get; set; } = string.Empty;
}
