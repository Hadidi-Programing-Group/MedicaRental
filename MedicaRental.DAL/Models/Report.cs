using MedicaRental.DAL.Context;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicaRental.DAL.Models
{
    public class Report : ISoftDeletable
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Statement { get; set; } = string.Empty;


        public bool IsSolved { get; set; } = false;

        //[ReadOnly(true)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

        public DateTime? SolveDate { get; set; }


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
