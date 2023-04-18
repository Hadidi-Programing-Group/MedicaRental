using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos.Report;


public record ReportDtos(Guid Id,string Name, string Statement,bool IsSolved,DateTime CreatedDate, DateTime? SolveDate, string ReportedId, string ReporteeId, Guid? MessageId, Guid? ReviewId, Guid? ItemId);
