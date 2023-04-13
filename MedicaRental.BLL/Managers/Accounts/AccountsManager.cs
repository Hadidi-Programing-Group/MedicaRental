﻿using MedicaRental.BLL.Dtos;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers;

public class AccountsManager : IAccountsManager
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;

    public AccountsManager(
        UserManager<AppUser> userManager,
        IConfiguration configuration
        )
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public Task DeleteAsync(AppUser newUser)
    {
        return _userManager.DeleteAsync(newUser);
    }

    public async Task<LoginStatusWithTokenDto> LoginAsync(LoginInfoDto loginInfoDto)
    {
        var user = await _userManager.FindByEmailAsync(loginInfoDto.Email);
        if (user is null)
            return new LoginStatusWithTokenDto(false, null, null);

        var isAuth = await _userManager.CheckPasswordAsync(user, loginInfoDto.Password);
        if (!isAuth)
            return new LoginStatusWithTokenDto(false, null, null);

        var ClaimsList = await _userManager.GetClaimsAsync(user);

        var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var SignInCreds = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256Signature);

        var expiry = DateTime.Now.AddDays(1);

        var token = new JwtSecurityToken(
            claims: ClaimsList,
            expires: expiry,
            signingCredentials: SignInCreds);

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenString = tokenHandler.WriteToken(token);

        return new LoginStatusWithTokenDto(true, tokenString, expiry);
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
