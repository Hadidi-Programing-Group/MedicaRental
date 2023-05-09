using MedicaRental.DAL.Context;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicaRental.DAL.Models
{
    public class Client
    {
        [ForeignKey("User")]
        [Key]
        public string Id { get; set; } = string.Empty;
        
        public string Ssn { get; set; } = string.Empty;
        
        public string Address { get; set; } = string.Empty;
        
        public bool IsGrantedRent { get; set; }
        
        public byte[]? NationalIdImage { get; set; }
        
        public byte[]? UnionCardImage { get; set; }

        public byte[]? ProfileImage { get; set; }

        public ICollection<Message> SentMessages { get; set; } = new List<Message>();
        
        public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();

        public ICollection<Report> Reports { get; set; } = new List<Report>();

        public ICollection<Item> ItemsForRent { get; set; } = new List<Item>();
        
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        public ICollection<RentOperation> RentOperations { get; set; } = new List<RentOperation>();

        public ICollection<CartItem> CartItems { get; set; } = new HashSet<CartItem>();

        public ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>();

        public AppUser? User { get; set; }

        [Range(0, 5)]
        public int Rating { get; set; } = 0;

        [NotMapped]
        public string Name { get => User?.Name ?? ""; }
    }
}
