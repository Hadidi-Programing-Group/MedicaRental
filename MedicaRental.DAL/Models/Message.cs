using MedicaRental.DAL.Context;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicaRental.DAL.Models
{
    public class Message : ISoftDeletable
    {
        public Guid Id { get; set; }
        
        public string Content { get; set; } = string.Empty;
        
        public DateTime Timestamp { get; set; }
        
        public bool IsDeleted { get; set; }

        [ForeignKey("Sender")]
        public string SenderId { get; set; } = string.Empty;
        public Client? Sender { get; set; }

        [ForeignKey("Receiver")]
        public string ReceiverId { get; set; } = string.Empty;
        public Client? Receiver { get; set; }
    }
}
