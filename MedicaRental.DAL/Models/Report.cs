using System.ComponentModel.DataAnnotations.Schema;

namespace MedicaRental.DAL.Models
{
    public class Report
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
        public int? ReviewId { get; set; }
        public Review? Review { get; set; }

        [ForeignKey("Item")]
        public int? ItemId { get; set; }
        public Item? Item { get; set; }
    }
}
