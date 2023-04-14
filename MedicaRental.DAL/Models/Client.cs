using MedicaRental.DAL.Context;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicaRental.DAL.Models
{
    public class Client
    {
        [ForeignKey("User")]
        public string Id { get; set; } = string.Empty;
        
        public string Ssn { get; set; } = string.Empty;
        
        public string Address { get; set; } = string.Empty;
        
        public bool IsGrantedRent { get; set; }
        
        public byte[]? NationalIdImage { get; set; }
        
        public byte[]? UnionCardImage { get; set; }

        public ICollection<Message> SentMessages { get; set; } = new List<Message>();
        
        public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();

        public ICollection<Report> Reports { get; set; } = new List<Report>();

        public ICollection<ItemPreviousRenters> RentedItems { get; set; } = new List<ItemPreviousRenters>();

        public AppUser? User { get; set; }

        [NotMapped]
        public float Rating { get => 0; }
    }
}
