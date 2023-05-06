using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Admin;
using MedicaRental.BLL.Dtos.Report;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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


    #region ByRaouf => For AdminPanel


    public async Task<IEnumerable<RoleMangerUserInfoDto>> GetAllAdminMod()
    {
        var adminUsers = await _userManager.GetUsersForClaimAsync(new Claim(ClaimTypes.Role, "admin"));
        var modUsers = await _userManager.GetUsersForClaimAsync(new Claim(ClaimTypes.Role, "Moderator"));

        var adminModlist = new List<RoleMangerUserInfoDto>();

        foreach (var user in adminUsers)
        {
            var temp = new RoleMangerUserInfoDto()
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email= user.Email,
                Role = UserRoles.Admin.ToString()
            };

            adminModlist.Add(temp);    
        }

        foreach (var user in modUsers)
        {
            var temp = new RoleMangerUserInfoDto()
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Role = UserRoles.Moderator.ToString()
            };

            adminModlist.Add(temp);
        }

        var orderedAdminModlist = adminModlist.OrderByDescending(user => user.Role == "admin" ? 0 : 1).ToList();

        return orderedAdminModlist;
    }



    public async Task<StatusDto> DeleteAdminMod(string id)
    {

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return new StatusDto(

               StatusCode: System.Net.HttpStatusCode.NotFound,
       StatusMessage: $"User with id {id} cannot be found");
        }

        // Prevents the deletion of OwnerAccount.
        if (user.Email == "admin@admin.com")
        {
            return new StatusDto(

               StatusCode: System.Net.HttpStatusCode.BadRequest,
       StatusMessage: $"Failed to delete User {user.Email} ");
        }


        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return new StatusDto(

               StatusCode: System.Net.HttpStatusCode.BadRequest,
       StatusMessage: $"Failed to delete User {user.Email} ");
        }

        return new StatusDto (

                StatusCode: System.Net.HttpStatusCode.OK,
        StatusMessage: $"User {user.Email} has been deleted");

    }
    #endregion


}
