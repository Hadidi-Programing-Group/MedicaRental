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

        public ICollection<Item> ItemsForRent { get; set; } = new List<Item>();
        
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        public ICollection<RentOperation> RentOperations { get; set; } = new List<RentOperation>();

        public AppUser? User { get; set; }

        [NotMapped]
        public int Rating { get => Reviews.Count == 0 ? 0 : (int)Reviews.Average(r => r.Rating); }

        [NotMapped]
        public string Name { get => User?.Name ?? ""; }
    }
}
