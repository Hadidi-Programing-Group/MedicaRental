using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos;


public record ReportDtos(Guid Id,string Name, string Statement,bool IsSolved,DateTime CreatedDate, DateTime? SolveDate, string ReportedId, string ReporteeId, Guid? MessageId, Guid? ReviewId, Guid? ItemId);

public record DetailedChatReportDto(Guid Id,string Name, string Statement,bool IsSolved,DateTime CreatedDate, DateTime? SolveDate, string ReportedId, string ReporteeId, Guid MessageId, string MessageContent, DateTime MessageTimeStamp);

public record ReportDto(Guid Id,string Name ,bool IsSolved,DateTime CreatedDate, DateTime? SolveDate, string ReportedName, string ReporteeName);

