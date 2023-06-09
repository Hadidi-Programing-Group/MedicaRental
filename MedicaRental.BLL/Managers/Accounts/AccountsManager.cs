﻿using MedicaRental.BL.MailService;
using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Account;
using MedicaRental.BLL.Dtos.Authentication;
using MedicaRental.BLL.Helpers;
using MedicaRental.BLL.Managers.Authentication;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
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
    private readonly IAuthManger _authManger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailSender _emailSender;

    public AccountsManager(
        UserManager<AppUser> userManager,
        IAuthManger authManger,
        IUnitOfWork unitOfWork,
        IEmailSender emailSender
    )
    {
        _userManager = userManager;
        this._authManger = authManger;
        _unitOfWork = unitOfWork;
        _emailSender = emailSender;
    }





    public async Task<StatusDto> BlockUserAsync(BlockUserInfoDto blockUserInfo)
    {
        var user = await _userManager.FindByIdAsync(blockUserInfo.Id);
        if (user is null)
            return new StatusDto(
                StatusMessage: $"User {blockUserInfo.Id} coudn't be found",
                StatusCode: System.Net.HttpStatusCode.NotFound
            );

        if (user.LockoutEnd > DateTimeOffset.Now)
            return new StatusDto(
                StatusCode: System.Net.HttpStatusCode.BadGateway,
                StatusMessage: $"User {user.Email} is already blocked"
            );

        var lockDate = await _userManager.SetLockoutEndDateAsync(user, blockUserInfo.EndDate);

        if (lockDate.Succeeded)
        {
            var items = await _unitOfWork.Items.FindAllAsync(predicate: i => i.SellerId == user.Id);
            try
            {
                _unitOfWork.Items.DeleteRange(items);
                _unitOfWork.Save();
                return new StatusDto(
                       StatusCode: System.Net.HttpStatusCode.OK,
                       StatusMessage: $"User {user.Email} is blocked untill {blockUserInfo.EndDate}"
                   );
            }
            catch
            {
                return new StatusDto(
                StatusMessage: $"User {user.Email} items coudn't be deleted",
                StatusCode: System.Net.HttpStatusCode.BadRequest
            );
            }




        }
        else
            return new StatusDto(
                StatusMessage: $"User {user.Email} coudn't be blocked",
                StatusCode: System.Net.HttpStatusCode.BadRequest
            );
    }

    public Task DeleteAsync(AppUser newUser)
    {
        return _userManager.DeleteAsync(newUser);
    }

    public async Task<StatusDto> ForgetPassword(ForgotPasswordDto forgotPasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
        if (user == null)
            return new StatusDto("Invalid Email", System.Net.HttpStatusCode.BadRequest);
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var param = new Dictionary<string, string?>
        {
            {"token", token },
            {"email", forgotPasswordDto.Email }
        };
        var callback = QueryHelpers.AddQueryString(forgotPasswordDto.ClientURI, param);

        var message = new EmailMessage(new string[] { user.Email }, "Reset Password", EmailHelpers.CreateEmailBody(callback));
        await _emailSender.SendEmailAsync(message);

        return new StatusDto("Reset info was sent to you email", System.Net.HttpStatusCode.OK);
    }

    public async Task<LoginStatusWithTokenDto> LoginAsync(LoginInfoDto loginInfoDto)
    {
        var authModel = new AuthModelDto();

        #region Checking Credentials
        var user = await _userManager.FindByEmailAsync(loginInfoDto.Email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, loginInfoDto.Password))
            return new LoginStatusWithTokenDto(
                "Email or password is not correct",
                System.Net.HttpStatusCode.NotFound,
                false,
                null,
                null,
                null,
                DateTime.UtcNow,
                null
            );

        var isBlocked = await _userManager.IsLockedOutAsync(user);
        if (isBlocked)
            return new LoginStatusWithTokenDto(
                "This Account is blocked",
                System.Net.HttpStatusCode.Unauthorized,
                false,
                null,
                null,
                null,
                DateTime.UtcNow,
                null
            );
        #endregion


        #region Creating the JWT
        var jwtSecurityToken = await _authManger.CreateJwtToken(user);

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenString = tokenHandler.WriteToken(jwtSecurityToken);

        authModel.IsAuthenticated = true;
        authModel.TokenString = tokenString;
        authModel.Email = user.Email;
        authModel.Username = user.UserName;
        authModel.ExpiresOn = jwtSecurityToken.ValidTo;
        #endregion

        #region RefreshToken Check/Create

        var activeRefreshToken = await _unitOfWork.RefreshToken.FindAsync(
            predicate: t => t.AppUserId == user.Id && t.RevokedOn == null
        );
        if (activeRefreshToken is not null)
        {
            authModel.RefreshToken = activeRefreshToken.Token;
            authModel.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
        }
        else
        {
            var refreshToken = _authManger.GenerateRefreshToken();
            authModel.RefreshToken = refreshToken.Token;
            authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
            //Save to Database
            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);
        }

        //var refreshToken = authManger.GenerateRefreshToken();
        //authModel.RefreshToken = refreshToken.Token;
        //authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
        ////Save to Database
        //user.RefreshTokens.Add(refreshToken);
        //await _userManager.UpdateAsync(user);

        #endregion


        var claims = await _userManager.GetClaimsAsync(user);

        return new LoginStatusWithTokenDto(
            "Login Successful",
            System.Net.HttpStatusCode.OK,
            authModel.IsAuthenticated,
            authModel.TokenString,
            authModel.ExpiresOn,
            authModel.RefreshToken,
            authModel.RefreshTokenExpiration,
            claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value
                ?? UserRoles.Client.ToString()
        );

        //return authModel;
    }

    public async Task<BaseUserRegisterStatusDto> RegisterNewUserAsync(
        BaseUserRegisterInfoDto baseUserRegisterInfoDto
    )
    {
        var newUser = new AppUser
        {
            FirstName = baseUserRegisterInfoDto.FirstName,
            LastName = baseUserRegisterInfoDto.LastName,
            Email = baseUserRegisterInfoDto.Email,
            UserName = baseUserRegisterInfoDto.Email,
            PhoneNumber = baseUserRegisterInfoDto.PhoneNumber,
        };

        var registerUserResult = await _userManager.CreateAsync(
            newUser,
            baseUserRegisterInfoDto.Password
        );
        if (!registerUserResult.Succeeded)
            return new BaseUserRegisterStatusDto(
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
            return new BaseUserRegisterStatusDto(
                isCreated: false,
                RegisterMessage: addingClaimsResult.Errors.First().Description,
                NewUser: null
            );

        return new BaseUserRegisterStatusDto(
            isCreated: true,
            RegisterMessage: "User Created Successfully",
            NewUser: newUser
        );
    }

    public async Task<StatusDto> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
        if (user == null)
            return new("Invalid Request", System.Net.HttpStatusCode.BadRequest);

        var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);
        if (!resetPassResult.Succeeded)
        {
            var errors = resetPassResult.Errors.Select(e => e.Description);

            return new(errors.FirstOrDefault() ?? "Can't Reset Password", System.Net.HttpStatusCode.BadRequest);
        }

        return new("Password Changed", System.Net.HttpStatusCode.OK);
    }

    public async Task<StatusDto> UnBlockUserAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return new StatusDto(
                StatusMessage: $"User {email} coudn't be found",
                StatusCode: System.Net.HttpStatusCode.NotFound
            );

        var lockDate = await _userManager.SetLockoutEndDateAsync(user, null);

        if (lockDate.Succeeded)
        {
            var items = await _unitOfWork.Items.FindAllAsync(predicate: i => i.SellerId == user.Id, ignoreQueryFilter: true);
            foreach (var item in items)
            {
                item.IsDeleted = false;
            }
            try
            {
                _unitOfWork.Items.UpdateRange(items);
                _unitOfWork.Save();
                return new StatusDto(
                   StatusCode: System.Net.HttpStatusCode.OK,
                   StatusMessage: $"User {email} is unblocked"
               );
            }
            catch
            {
                return new StatusDto(
                StatusMessage: $"User {user.Email} items coudn't be restored",
                StatusCode: System.Net.HttpStatusCode.BadRequest
            );
            }


        }
        else
            return new StatusDto(
                StatusMessage: $"User {email} coudn't be unblocked",
                StatusCode: System.Net.HttpStatusCode.BadRequest
            );
    }

    #region OldLogin(No-RefreshToken)


    //public async Task<LoginStatusWithTokenDto> LoginAsync(LoginInfoDto loginInfoDto)
    //{
    //    var user = await _userManager.FindByEmailAsync(loginInfoDto.Email);
    //    if (user is null)
    //        return new LoginStatusWithTokenDto("Email or password is not correct", System.Net.HttpStatusCode.NotFound, false, null, null);

    //    var isAuth = await _userManager.CheckPasswordAsync(user, loginInfoDto.Password);
    //    if (!isAuth)
    //        return new LoginStatusWithTokenDto("Email or password is not correct", System.Net.HttpStatusCode.NotFound, false, null, null);

    //    var isBlocked = await _userManager.IsLockedOutAsync(user);
    //    if (isBlocked)
    //        return new LoginStatusWithTokenDto("This Account is blocked", System.Net.HttpStatusCode.Unauthorized, false, null, null);


    //    var ClaimsList = await _userManager.GetClaimsAsync(user);

    //    var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

    //    var SignInCreds = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256Signature);

    //    var expiry = DateTime.UtcNow.AddDays(1);

    //    var token = new JwtSecurityToken(
    //        claims: ClaimsList,
    //        expires: expiry,
    //        signingCredentials: SignInCreds);

    //    var tokenHandler = new JwtSecurityTokenHandler();
    //    var tokenString = tokenHandler.WriteToken(token);

    //    return new LoginStatusWithTokenDto("Login Successful", System.Net.HttpStatusCode.OK, true, tokenString, expiry);
    //}

    #endregion
}
