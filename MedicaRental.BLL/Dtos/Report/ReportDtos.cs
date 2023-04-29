using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos;

public enum ReportCategory
{
    Items,
    Reviews,
    Chats
}

public record DetailedReportDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Statement { get; set; } = string.Empty;
    public bool IsSolved { get; set; }
    public string CreatedDate { get; set; } = string.Empty;
    public string? SolveDate { get; set; } = string.Empty;
    public string ReportedId { get; set; } = string.Empty;
    public string ReporterId { get; set; } = string.Empty;
    public string ReportedName { get; set; } = string.Empty;
    public string ReporterName { get; set; } = string.Empty;
    public Guid ContentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string ContentTimeStamp { get; set; } = string.Empty;
    public string ReportCategory { get; set; } = string.Empty;
    public bool IsReportedUserBlocked { get; set; }
    public bool IsContentDeleted { get; set; }
    public IEnumerable<ReportActionDto> ReportActions { get; set; } = new HashSet<ReportActionDto>();
}

public record ReportActionDto(string Action, DateTime CreateDate, string TakenBy);

public record ReportDto(Guid Id, string Name, bool IsSolved, string CreatedDate, string? SolveDate, string ReportedName, string ReporterName);

