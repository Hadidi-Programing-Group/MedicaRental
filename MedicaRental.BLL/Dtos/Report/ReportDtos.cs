using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos;



public record DetailedReportDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Statement { get; set; } = string.Empty;
    public bool IsSolved { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? SolveDate { get; set; }
    public string ReportedId { get; set; } = string.Empty;
    public string ReporterId { get; set; } = string.Empty;
    public string ReportedName { get; set; } = string.Empty;
    public string ReporterName { get; set; } = string.Empty;
    public Guid ContentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime ContentTimeStamp { get; set; }
}

public record ReportDto(Guid Id, string Name, bool IsSolved, DateTime CreatedDate, DateTime? SolveDate, string ReportedName, string ReporterName);

