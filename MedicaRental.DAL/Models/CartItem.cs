using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Models;

public class CartItem
{
    public Guid Id { get; set; }

    [ForeignKey(nameof(Item))]
    public Guid ItemId { get; set; }
    public Item? Item { get; set; }

    [ForeignKey(nameof(Client))]
    public string ClientId { get; set; } = string.Empty;
    public Client? Client { get; set; }
}
