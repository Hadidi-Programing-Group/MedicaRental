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
                Statement : report.Statement,
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

    public async Task<IEnumerable<ReportDtos>> GetChatReportsAsync()
    {
        var reports = await _unitOfWork.Reports.FindAllAsync( 
            predicate: report=>report.MessageId != null
            );
        var reportDtos = new List<ReportDtos>(); 

        if (reports != null)
        {
            foreach( var report in reports)
            {
                var reportDto = new ReportDtos(report.Id, report.Statement, report.ReportedId, report.ReporteeId,
                (Guid)report.MessageId, Guid.Empty, Guid.Empty);
                reportDtos.Add(reportDto);

            }
        }
        return reportDtos;

    }

    public async Task<IEnumerable<ReportDtos>> GetItemReportsAsync()
    {
        var reports = await _unitOfWork.Reports.FindAllAsync(
             predicate: report => report.ItemId != null
             );
        var reportDtos = new List<ReportDtos>();

        if (reports != null)
        {
            foreach (var report in reports)
            {
                var reportDto = new ReportDtos(report.Id, report.Statement, report.ReportedId, report.ReporteeId,
                Guid.Empty, Guid.Empty,(Guid)report.ItemId );
                reportDtos.Add(reportDto);

            }
        }
        return reportDtos;
    }

    public async Task<IEnumerable<ReportDtos>> GetReviewReportsAsync()
    {
        var reports = await _unitOfWork.Reports.FindAllAsync(
             predicate: report => report.ReviewId != null
             );
        var reportDtos = new List<ReportDtos>();

        if (reports != null)
        {
            foreach (var report in reports)
            {
                var reportDto = new ReportDtos(report.Id, report.Statement, report.ReportedId, report.ReporteeId,
                 Guid.Empty,(Guid)report.ReviewId ,Guid.Empty);
                reportDtos.Add(reportDto);

            }
        }
        return reportDtos;
    }

        public async Task<InsertReportStatusDto> InsertNewReport(InsertReportDtos insertReport)
        {
        Console.WriteLine("Dataaaaaaaaaaaaaaaaaaaaaaaa1");
            if (insertReport == null)
            {
                return new InsertReportStatusDto(false, null, "InsertReportDtos object is null.");
            }

            
            var report = new Report
            {
                Statement = insertReport.Statement,
                ReportedId = insertReport.ReportedId,
                
                ReporteeId = insertReport.ReporteeId,
               
                MessageId = insertReport.MessageId,
                ReviewId = insertReport.ReviewId,
                ItemId = insertReport.ItemId
            };

            
            _unitOfWork.Reports.AddAsync(report);
       
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
