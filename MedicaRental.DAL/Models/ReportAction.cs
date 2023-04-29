using MedicaRental.DAL.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Models
{
    public class ReportAction
    {
        public Guid Id { get; set; }

        public string Action { get; set; } = string.Empty;

        public DateTime CreateTime { get; set; } = DateTime.Now;



        [ForeignKey(nameof(Report))]
        public Guid ReportId { get; set; }
        public Report? Report { get; set; }

        [ForeignKey(nameof(AppUser))]
        public string AdminId {get; set;} = string.Empty;
        public AppUser? AppUser { get; set; }
    }
}
