using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Admin;
using MedicaRental.DAL.Context;
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

    public AdminsManager(UserManager<AppUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
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
}
