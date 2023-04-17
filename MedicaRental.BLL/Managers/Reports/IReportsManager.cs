using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Report;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers;

public interface IReportsManager
{
    
    Task<IEnumerable<ReportDtos>> GetChatReportsAsync();
    Task<IEnumerable<ReportDtos>> GetReviewReportsAsync();
    Task<IEnumerable<ReportDtos>> GetItemReportsAsync();
    Task<ReportDtos?> GetByIdAsync(Guid? id);
    Task<DeleteReportStatusDto> DeleteByIdAsync(Guid id);
    Task<InsertReportStatusDto> InsertNewReport(InsertReportDtos insertReport);

}
