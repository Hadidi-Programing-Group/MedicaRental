using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Admin;
using MedicaRental.BLL.Dtos.Report;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers;

public class AdminsManager : IAdminsManager
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;

  

    public AdminsManager(UserManager<AppUser> userManager, IConfiguration configuration, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _configuration = configuration;
        _unitOfWork = unitOfWork;

    }

    public async Task<UserRoleUpdateStatusDto> UpdateUserRoleAsync(
            UserRoleUpdateDto userRoleUpdateDto
        )
    {
        // Check if user exists
        var user = await _userManager.FindByIdAsync(userRoleUpdateDto.UserId);
        if (user == null)
            return new UserRoleUpdateStatusDto(
                IsUpdated: false,
                UpdateMessage: $"User with id {userRoleUpdateDto.UserId} does not exist",
                StatusCode: HttpStatusCode.NotFound
            );

        // Check if role is valid
        if (!Enum.IsDefined(typeof(UserRoles), userRoleUpdateDto.NewRole))
            return new UserRoleUpdateStatusDto(
                IsUpdated: false,
                UpdateMessage: $"Invalid user role: {userRoleUpdateDto.NewRole}",
                StatusCode: HttpStatusCode.BadRequest
            );

        // Update user's role claim
        //      Get List of claims
        var existingRoleClaims = await _userManager.GetClaimsAsync(user);
        //      Filter to reach Role claim
        var CurrentRoleClaim = existingRoleClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        if (CurrentRoleClaim == null)
        {
            // User does not have a role claim, so add a new one
            CurrentRoleClaim = new Claim(ClaimTypes.Role, userRoleUpdateDto.NewRole.ToString());
            var addClaimResult = await _userManager.AddClaimAsync(user, CurrentRoleClaim);
            if (!addClaimResult.Succeeded)
                return new UserRoleUpdateStatusDto(
                    IsUpdated: false,
                    UpdateMessage: $"Failed to add new role claim for user {user.UserName}",
                    StatusCode: HttpStatusCode.InternalServerError
                );
        }
        else
        {
            // User already has a role claim, so update its value
            var NewRoleClaim = new Claim(ClaimTypes.Role, userRoleUpdateDto.NewRole.ToString());
            var replaceClaimResult = await _userManager.ReplaceClaimAsync(
                user,
                CurrentRoleClaim,
                NewRoleClaim
            );
            if (!replaceClaimResult.Succeeded)
                return new UserRoleUpdateStatusDto(
                    IsUpdated: false,
                    UpdateMessage: $"Failed to update role claim for user {user.UserName}",
                    StatusCode: HttpStatusCode.InternalServerError
                );
        }

        return new UserRoleUpdateStatusDto(
            IsUpdated: true,
            UpdateMessage: $"User role updated to {userRoleUpdateDto.NewRole} successfully",
            StatusCode: HttpStatusCode.OK
        );


    }

    public async Task<ReportUpdateStatusDto> UpdateReportStatus(ReportUpdateDto reportUpdateDto)
    {
        var report = await _unitOfWork.Reports.FindAsync(
            predicate: (p) => p.Id == reportUpdateDto.ReportId
            );

        // Check if report exists 
        if (report == null)
            return new ReportUpdateStatusDto(
                    IsUpdated: false,
                    UpdateMessage: $"User with id {reportUpdateDto.ReportId} does not exist",
                    StatusCode: HttpStatusCode.NotFound
                );

        // Check if IsSolved is bool 
        if (!bool.TryParse(reportUpdateDto.IsSolved.ToString(), out var isSolvedBool))
        {
            return new ReportUpdateStatusDto(
                IsUpdated: false,
                UpdateMessage: "Invalid value for IsSolved parameter",
                StatusCode: HttpStatusCode.BadRequest
            );
        }

        // Check if IsSolved already has that value
        if (report.IsSolved == reportUpdateDto.IsSolved)
        {
            return new ReportUpdateStatusDto(
                IsUpdated: false,
                UpdateMessage: $"Report with id {reportUpdateDto.ReportId} already has the status {report.IsSolved}",
                StatusCode: HttpStatusCode.OK
            );
        }

        try
        {
            report.IsSolved = reportUpdateDto.IsSolved;

            _unitOfWork.Reports.Update(report);
            _unitOfWork.Save();

            return new ReportUpdateStatusDto(
                IsUpdated: true,
                UpdateMessage: $"Report with id {reportUpdateDto.ReportId} has been updated to {reportUpdateDto.IsSolved}",
                StatusCode: HttpStatusCode.OK
            );


        }
        catch
        {

            return new ReportUpdateStatusDto(
                IsUpdated: false,
                UpdateMessage: $"Report with id {reportUpdateDto.ReportId} wasn't updated due to a server error ",
                StatusCode: HttpStatusCode.InternalServerError
            );

        }
        



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
                Id: report.Id,
                Name: report.Name,
                Statement: report.Statement,
                IsSolved: report.IsSolved,
                CreatedDate: report.CreatedDate,
                SolveDate: report.SolveDate,
                ReportedId: report.ReportedId,
                ReporteeId: report.ReporteeId,
                MessageId: report.MessageId,
                ReviewId: report.ReviewId,
                ItemId: report.ItemId
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
            predicate: report => report.MessageId != null
            );
        var reportDtos = new List<ReportDtos>();

        if (reports != null)
        {
            foreach (var report in reports)
            {
                var reportDto = new ReportDtos(report.Id, Name: report.Name,
                Statement: report.Statement,
                IsSolved: report.IsSolved,
                CreatedDate: report.CreatedDate,
                SolveDate: report.SolveDate, report.ReportedId, report.ReporteeId,
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
                var reportDto = new ReportDtos(report.Id, Name: report.Name,
                Statement: report.Statement,
                IsSolved: report.IsSolved,
                CreatedDate: report.CreatedDate,
                SolveDate: report.SolveDate, report.ReportedId, report.ReporteeId,
                Guid.Empty, Guid.Empty, (Guid)report.ItemId);
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
                var reportDto = new ReportDtos(report.Id, Name: report.Name,
                Statement: report.Statement,
                IsSolved: report.IsSolved,
                CreatedDate: report.CreatedDate,
                SolveDate: report.SolveDate, report.ReportedId, report.ReporteeId,
                 Guid.Empty, (Guid)report.ReviewId, Guid.Empty);
                reportDtos.Add(reportDto);

            }
        }
        return reportDtos;
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
