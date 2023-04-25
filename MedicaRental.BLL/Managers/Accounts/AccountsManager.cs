using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Authentication;
using MedicaRental.BLL.Managers.Authentication;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using Microsoft.AspNetCore.Identity;
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
    private readonly IAuthManger authManger;
    private readonly IConfiguration _configuration;

    public AccountsManager(
        UserManager<AppUser> userManager,
        IAuthManger authManger,
        IConfiguration configuration
    )
    {
        _userManager = userManager;
        this.authManger = authManger;
        _configuration = configuration;
    }

    public async Task<StatusDto> BlockUserAsync(BlockUserInfoDto blockUserInfo)
    {
        var user = await _userManager.FindByEmailAsync(blockUserInfo.Email);
        if (user is null)
            return new StatusDto(
                StatusMessage: $"User {blockUserInfo.Email} coudn't be found",
                StatusCode: System.Net.HttpStatusCode.NotFound
            );

        var lockDate = await _userManager.SetLockoutEndDateAsync(user, blockUserInfo.EndDate);

        if (lockDate.Succeeded)
            return new StatusDto(
                StatusCode: System.Net.HttpStatusCode.OK,
                StatusMessage: $"User {blockUserInfo.Email} is blocked untill {blockUserInfo.EndDate}"
            );
        else
            return new StatusDto(
                StatusMessage: $"User {blockUserInfo.Email} coudn't be blocked",
                StatusCode: System.Net.HttpStatusCode.BadRequest
            );
    }

    public Task DeleteAsync(AppUser newUser)
    {
        return _userManager.DeleteAsync(newUser);
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
                DateTime.UtcNow
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
                DateTime.UtcNow
            );
        #endregion


        #region Creating the JWT
        var jwtSecurityToken = await authManger.CreateJwtToken(user);

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenString = tokenHandler.WriteToken(jwtSecurityToken);

        authModel.IsAuthenticated = true;
        authModel.TokenString = tokenString;
        authModel.Email = user.Email;
        authModel.Username = user.UserName;
        authModel.ExpiresOn = jwtSecurityToken.ValidTo;
        #endregion

        #region RefreshToken Check/Create

        //if (user.RefreshTokens.Any(t => t.IsActive))
        //{
        //    var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
        //    authModel.RefreshToken = activeRefreshToken.Token;
        //    authModel.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
        //}
        //else
        //{
        //    var refreshToken = authManger.GenerateRefreshToken();
        //    authModel.RefreshToken = refreshToken.Token;
        //    authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
        //    //Save to Database
        //    user.RefreshTokens.Add(refreshToken);
        //    await _userManager.UpdateAsync(user);
        //}


        var refreshToken = authManger.GenerateRefreshToken();
        authModel.RefreshToken = refreshToken.Token;
        authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
        //Save to Database
        user.RefreshTokens.Add(refreshToken);
        await _userManager.UpdateAsync(user);

        #endregion





        return new LoginStatusWithTokenDto(
            "Login Successful",
            System.Net.HttpStatusCode.OK,
            authModel.IsAuthenticated,
            authModel.TokenString,
            authModel.ExpiresOn,
            authModel.RefreshToken,
            authModel.RefreshTokenExpiration
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
            return new StatusDto(
                StatusCode: System.Net.HttpStatusCode.OK,
                StatusMessage: $"User {email} is unblocked"
            );
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

    //    var expiry = DateTime.Now.AddDays(1);

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
