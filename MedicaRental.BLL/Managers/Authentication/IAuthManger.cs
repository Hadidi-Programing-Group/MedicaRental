using MedicaRental.BLL.Dtos.Authentication;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers.Authentication
{
    public interface IAuthManger
    {
        Task<AuthModelDto> RenewTokens(string token);
        Task<bool> RevokeTokenAsync(string token);


        #region Edited 
        Task<JwtSecurityToken> CreateJwtToken(AppUser user);

        RefreshToken GenerateRefreshToken();
        #endregion
    }
}
