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
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

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
                                     .Include(r => r.ReportActions).ThenInclude(ra => ra.AppUser)
            );

        if (report is null)
            return null;


        var detailedReportDto = new DetailedReportDto()
        {
            Id = report.Id,
            Name = report.Name,
            Statement = report.Statement,
            IsSolved = report.IsSolved,
            CreatedDate = report.CreatedDate.ToString("o"),
            SolveDate = report.SolveDate?.ToString("o") ?? "",
            ReportedId = report.ReportedId,
            ReporterId = report.ReporterId,
            ReportedName = report.Reported.Name,
            ReporterName = report.Reported.Name,
            IsReportedUserBlocked = report.Reported.User.LockoutEnd > DateTimeOffset.Now,
            ReportActions = report.ReportActions.Select(ra => new ReportActionDto(
                ra.Action, ra.CreateTime, ra.AppUser.Name 
                ))
        };

        if (report.MessageId is not null)
        {
            var message = await _unitOfWork.Messages.FindAsync(
                ignoreQueryFilter: true,
                predicate: m => m.Id == report.MessageId
                );

            if (message is null) return null;

            detailedReportDto.ContentId = message.Id;
            detailedReportDto.IsContentDeleted = message.IsDeleted;
            detailedReportDto.Content = message.Content;
            detailedReportDto.ContentTimeStamp = message.Timestamp.ToString("o");
            detailedReportDto.ReportCategory = ReportCategory.Chats.ToString();
        }


        else if (report.ItemId is not null)
        {
            var item = await _unitOfWork.Items.FindAsync(
                ignoreQueryFilter: true,
                predicate: m => m.Id == report.ItemId
                );

            if (item is null) return null;

            detailedReportDto.ContentId = item.Id;
            detailedReportDto.Content = item.Name;
            detailedReportDto.IsContentDeleted = item.IsDeleted;
            detailedReportDto.ContentTimeStamp = item.CreationDate.ToString("o");
            detailedReportDto.ReportCategory = ReportCategory.Items.ToString();
        }

        else if (report.ReviewId is not null)
        {
            var review = await _unitOfWork.Reviews.FindAsync(
                ignoreQueryFilter: true,
                predicate: m => m.Id == report.ReviewId
                );

            if (review is null) return null;

            detailedReportDto.ContentId = review.Id;
            detailedReportDto.Content = review.ClientReview;
            detailedReportDto.IsContentDeleted = review.IsDeleted;
            detailedReportDto.ContentTimeStamp = review.CreateDate.ToString("o");
            detailedReportDto.ReportCategory = ReportCategory.Reviews.ToString();
        }
        return detailedReportDto;
    }

    public async Task<PageDto<ReportDto>?> GetChatReportsAsync(int page)
    {
        try
        {

            var reports = await _unitOfWork.Reports.FindAllAsync(
                include: ReportHelper.ReportListInclude,
                selector: ReportHelper.ReportListSeletor,
                predicate: report => report.MessageId != null,
                orderBy: r => r.OrderBy(r => r.IsSolved),
                skip: SharedHelper.Take * (page - 1),
                take: SharedHelper.Take
                );

            var count = await _unitOfWork.Reports.GetCountAsync(
                predicate: report => report.MessageId != null);

            return new(reports, count);
        }
        catch
        {
            return null;
        }

    }

    public async Task<PageDto<ReportDto>?> GetItemReportsAsync(int page)
    {
        try
        {

            var reports = await _unitOfWork.Reports.FindAllAsync(
            include: ReportHelper.ReportListInclude,
            selector: ReportHelper.ReportListSeletor,
            predicate: report => report.ItemId != null,
            orderBy: r => r.OrderBy(r => r.IsSolved),
            skip: SharedHelper.Take * (page - 1),
            take: SharedHelper.Take
            );

            var count = await _unitOfWork.Reports.GetCountAsync(
                 predicate: report => report.ItemId != null);

            return new(reports, count);
        }
        catch
        {
            return null;
        }
    }

    public async Task<PageDto<ReportDto>?> GetReviewReportsAsync(int page)
    {
        try
        {
            var reports = await _unitOfWork.Reports.FindAllAsync(
            include: ReportHelper.ReportListInclude,
            selector: ReportHelper.ReportListSeletor,
            predicate: report => report.ReviewId != null,
            orderBy: r => r.OrderBy(r => r.IsSolved),
            skip: SharedHelper.Take * (page - 1),
            take: SharedHelper.Take
            );


            var count = await _unitOfWork.Reports.GetCountAsync(
                 predicate: report => report.ReviewId != null);

            return new(reports, count);
        }
        catch
        {
            return null;
        }
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

    public async Task<StatusDto> MarkAsSolvedAsync(Guid id)
    {
        var report = await _unitOfWork.Reports.FindAsync(r => r.Id == id);
        if (report is null)
            return new StatusDto("Report not found", System.Net.HttpStatusCode.NotFound);

        report.IsSolved = !report.IsSolved;

        string actionTaken = report.IsSolved ? "Report was marked as solved" : "Report was re-opened";
        var updateResult =  _unitOfWork.Reports.Update(report);
        
        try
        {
            _unitOfWork.Save();
            return new StatusDto(actionTaken, System.Net.HttpStatusCode.OK);
        }
        catch
        {
            return new StatusDto("Report couldn't be updated", System.Net.HttpStatusCode.InternalServerError);
        }
    }
}
