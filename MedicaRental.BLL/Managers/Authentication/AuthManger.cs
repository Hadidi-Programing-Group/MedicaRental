using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Authentication;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers.Authentication
{
    public class AuthManger : IAuthManger
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork unitOfWork;

        //private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;

        public AuthManger(
            UserManager<AppUser> userManager,
            IOptions<JWT> jwt,
            IUnitOfWork unitOfWork
        )
        {
            _userManager = userManager;
            this.unitOfWork = unitOfWork;
            //_roleManager = roleManager;
            _jwt = jwt.Value;
        }

        #region To be Removed Or Refactored


        public async Task<JwtSecurityToken> CreateJwtToken(AppUser user)
        {
            var ClaimsList = await _userManager.GetClaimsAsync(user);

            var symmetricSecurityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwt.Secret)
            );
            var signingCredentials = new SigningCredentials(
                symmetricSecurityKey,
                SecurityAlgorithms.HmacSha256
            );

            var expiry = DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: ClaimsList,
                expires: expiry,
                signingCredentials: signingCredentials
            );

            return jwtSecurityToken;
        }

        public async Task<AuthModelDto> RenewTokens(string token)
        {
            var authModel = new AuthModelDto();

            // 1- Check if any user have this RefreshToken
            var user = await _userManager.Users.SingleOrDefaultAsync(
                u => u.RefreshTokens.Any(t => t.Token == token)
            );

            if (user == null)
            {
                authModel.Message = "Invalid token";
                return authModel;
            }

            //2- Retrive RefreshToken object, to check `IsActive`

            /*
                Could have gotten the data from dbcontext but we already have it
                In memory,so we use that for less trips.
             */
            var refreshToken = user.RefreshTokens.SingleOrDefault(t => t.Token == token);

            if (!refreshToken.IsActive)
            {
                // Inactive due to , expired or revoked.

                // Revoke any refreshTokens that are still active 
                // (Secures Account against Hack)
                //foreach (var refreshtoken in user.RefreshTokens)
                //{
                //    refreshtoken.RevokedOn = DateTime.UtcNow;
                //}

                //await _userManager.UpdateAsync(user);

                authModel.Message = "Inactive token, Re-login";

                return authModel;

            }

            // Revoked oldRefreshToken

            //refreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var jwtSecurityToken = await CreateJwtToken(user);

            authModel.IsAuthenticated = true;

            authModel.TokenString = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;

            // This Access Token ExpiryDate will tell us when to call this action.
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;

            authModel.RefreshToken = newRefreshToken.Token;
            authModel.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

            return authModel;
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(
                u => u.RefreshTokens.Any(t => t.Token == token)
            );

            if (user == null)
                return false;

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
                return false;


            foreach (var refreshtoken in user.RefreshTokens)
            {
                refreshtoken.RevokedOn = DateTime.UtcNow;
            }

            await _userManager.UpdateAsync(user);

            //refreshToken.RevokedOn = DateTime.UtcNow;
            //await _userManager.UpdateAsync(user);

            return true;
        }

        #endregion

        #region Good Code


        public RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using var generator = new RNGCryptoServiceProvider();

            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow
            };
        }

        #endregion
    }
}
