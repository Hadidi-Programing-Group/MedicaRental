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


    public async Task<DetailedReportDto?> GetByIdAsync(Guid? id)
    {

        var report = await _unitOfWork.Reports.FindAsync(
            predicate: (p) => p.Id == id,
            include: source => source.Include(r => r.Reported).ThenInclude(re => re.User)
                                     .Include(r => r.Reporter).ThenInclude(re => re.User)
            );

        if (report is null)
            return null;

        var detailedReportDto = new DetailedReportDto()
        {
            Id = report.Id,
            Name = report.Name,
            Statement = report.Statement,
            IsSolved = report.IsSolved,
            CreatedDate = report.CreatedDate,
            SolveDate = report.SolveDate,
            ReportedId = report.ReportedId,
            ReporterId = report.ReporterId,
            ReportedName = report.Reported.Name,
            ReporterName = report.Reported.Name,
        };

        if (report.MessageId is not null)
        {
            var message = await _unitOfWork.Messages.FindAsync(
                predicate: m => m.Id == report.MessageId
                );

            if (message is null) return null;

            detailedReportDto.ContentId = message.Id;
            detailedReportDto.Content = message.Content;
            detailedReportDto.ContentTimeStamp = message.Timestamp;
        }


        else if (report.ItemId is not null)
        {
            var item = await _unitOfWork.Items.FindAsync(
                predicate: m => m.Id == report.ItemId
                );

            if (item is null) return null;

            detailedReportDto.ContentId = item.Id;
            detailedReportDto.Content = item.Name;
            detailedReportDto.ContentTimeStamp = item.CreationDate;
        }

        else if (report.ReviewId is not null)
        {
            var review = await _unitOfWork.Reviews.FindAsync(
                predicate: m => m.Id == report.ReviewId
                );

            if (review is null) return null;

            detailedReportDto.ContentId = review.Id;
            detailedReportDto.Content = review.ClientReview;
            detailedReportDto.ContentTimeStamp = review.CreateDate;
        }
        return detailedReportDto;
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
                
                ReporterId = insertReport.ReporteeId,
               
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
