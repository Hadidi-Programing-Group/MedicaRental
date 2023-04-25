using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Report;
using MedicaRental.DAL.UnitOfWork;
using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedicaRental.BLL.Helpers;
using Microsoft.EntityFrameworkCore;

namespace MedicaRental.BLL.Managers;

public class ReportsManager : IReportsManager
{
    private readonly IUnitOfWork _unitOfWork;

    public ReportsManager(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteReportStatusDto> DeleteByIdAsync(Guid id)
    {
     

        await _unitOfWork.Reports.DeleteOneById(id);
        try
        {
            _unitOfWork.Save();
            return new DeleteReportStatusDto(
                isDeleted: true,
                StatusMessage: "Report was deleted successfully"
                );
        }
        catch
        {
            return new DeleteReportStatusDto(
                isDeleted: false,
                StatusMessage: "Coudn't remove Report"
                );
        }


    }


    public async Task<ReportDtos?> GetByIdAsync(Guid? id)
    {

        var report = await _unitOfWork.Reports.FindAsync(
            predicate: (p) => p.Id == id
            );
        if (report != null)
        {
            return new ReportDtos(
                Id : report.Id,
                Name : report.Name,
                Statement : report.Statement,
                IsSolved : report.IsSolved,
                CreatedDate : report.CreatedDate,
                SolveDate : report.SolveDate,
                ReportedId : report.ReportedId,
                ReporteeId : report.ReporteeId,
                MessageId : report.MessageId,
                ReviewId : report.ReviewId,
                ItemId : report.ItemId
                );
 
           
        }
        else
        {
            return null;
        }


    }

    public async Task<IEnumerable<ReportDto>> GetChatReportsAsync()
    {
        var reports = await _unitOfWork.Reports.FindAllAsync(
            include: ReportHelper.ReportListInclude,
            selector: ReportHelper.ReportListSeletor,
            predicate: report => report.MessageId != null,
            orderBy: r => r.OrderBy(r => r.IsSolved)
            ) ;

        return reports;

    }

    public async Task<IEnumerable<ReportDto>> GetItemReportsAsync()
    {

        var reports = await _unitOfWork.Reports.FindAllAsync(
            include: ReportHelper.ReportListInclude,
            selector: ReportHelper.ReportListSeletor,
            predicate: report => report.ItemId != null,
            orderBy: r => r.OrderBy(r => r.IsSolved)
            );

        return reports;
    }

    public async Task<IEnumerable<ReportDto>> GetReviewReportsAsync()
    {
        var reports = await _unitOfWork.Reports.FindAllAsync(
            include: ReportHelper.ReportListInclude,
            selector: ReportHelper.ReportListSeletor,
            predicate: report => report.ReviewId != null,
            orderBy: r => r.OrderBy(r => r.IsSolved)
            );

        return reports;
    }

        public async Task<InsertReportStatusDto> InsertNewReport(InsertReportDtos insertReport)
        {
            if (insertReport == null)
            {
                return new InsertReportStatusDto(false, null, "InsertReportDtos object is null.");
            }

            
            var report = new Report
            {
                Name = insertReport.Name,
                Statement = insertReport.Statement,
                ReportedId = insertReport.ReportedId,
                
                ReporteeId = insertReport.ReporteeId,
               
                MessageId = insertReport.MessageId,
                ReviewId = insertReport.ReviewId,
                ItemId = insertReport.ItemId
            };

            

            await _unitOfWork.Reports.AddAsync(report);
        try
        {
            _unitOfWork.Save();

        }
        catch
        {
            return new InsertReportStatusDto(false, null, "Report not Inserted.");
        }
          

           
            return new InsertReportStatusDto(true, report.Id, "Report inserted successfully.");
        }

 
}
