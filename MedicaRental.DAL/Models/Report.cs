using MedicaRental.DAL.Context;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicaRental.DAL.Models
{
    public class Report : ISoftDeletable
    {
        public Guid Id { get; set; }

        public string Statement { get; set; } = string.Empty;
        
        public bool IsDeleted { get; set; }

        [ForeignKey("Reported")]
        public string ReportedId { get; set; } = string.Empty;
        public Client? Reported { get; set; }

        [ForeignKey("Reportee")]
        public string ReporteeId { get; set; } = string.Empty;
        public Client? Reportee { get; set; }

        [ForeignKey("Message")]
        public Guid? MessageId { get; set; }
        public Message? Message { get; set; }

        [ForeignKey("Review")]
        public Guid? ReviewId { get; set; }
        public Review? Review { get; set; }

        [ForeignKey("Item")]
        public Guid? ItemId { get; set; }
        public Item? Item { get; set; }
    }
}
