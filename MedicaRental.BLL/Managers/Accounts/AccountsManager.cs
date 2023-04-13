using MedicaRental.BLL.Dtos;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers;

public class AccountsManager : IAccountsManager
{
    private readonly UserManager<AppUser> _userManager;

    public AccountsManager(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public Task DeleteAsync(AppUser newUser)
    {
        return _userManager.DeleteAsync(newUser);
    }

    public async Task<BaseUserRegisterStatusDto> RegisterNewUserAsync(BaseUserRegisterInfoDto baseUserRegisterInfoDto)
    {
        var newUser = new AppUser
        {
            FirstName = baseUserRegisterInfoDto.FirstName,
            LastName = baseUserRegisterInfoDto.LastName,
            Email = baseUserRegisterInfoDto.Email,
            UserName = baseUserRegisterInfoDto.Email,
            PhoneNumber = baseUserRegisterInfoDto.PhoneNumber,
        };

        var registerUserResult = await _userManager.CreateAsync(newUser, baseUserRegisterInfoDto.Password);
        if (!registerUserResult.Succeeded)
            return new BaseUserRegisterStatusDto
            (
                isCreated: false,
                RegisterMessage: registerUserResult.Errors.First().Description,
                NewUser: null
            );

        var ClaimsList = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, newUser.Id),
            new Claim(ClaimTypes.Email, newUser.Email),
            new Claim(ClaimTypes.GivenName, newUser.FirstName),
            new Claim(ClaimTypes.Surname, newUser.LastName),
            new Claim(ClaimTypes.Role, baseUserRegisterInfoDto.UserRole.ToString()),
        };

        var addingClaimsResult = await _userManager.AddClaimsAsync(newUser, ClaimsList);
        if (!addingClaimsResult.Succeeded)
            return new BaseUserRegisterStatusDto
            (
                isCreated: false,
                RegisterMessage: addingClaimsResult.Errors.First().Description,
                NewUser: null
            );

        return new BaseUserRegisterStatusDto
            (
                isCreated: true,
                RegisterMessage: "User Created Successfully",
                NewUser: newUser
            );
    }
}
