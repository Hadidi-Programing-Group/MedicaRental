using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Authentication;
using MedicaRental.BLL.Managers;
using MedicaRental.BLL.Managers.Authentication;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace MedicaRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IClientsManager _clientsManager;
        private readonly IAccountsManager _accountsManager;
        private readonly IAuthManger authManger;

        public AccountsController(
            IClientsManager clientsManager,
            IAccountsManager accountsManager,
            IAuthManger authManger
        )
        {
            _clientsManager = clientsManager;
            _accountsManager = accountsManager;
            this.authManger = authManger;
        }

        [HttpPost]
        [Route("/Register")]
        public async Task<ActionResult> RegisterAsync(ClientRegisterInfoDto clientRegisterInfoDto)
        {
            ClientRegisterStatusDto clientRegisterStatus =
                await _clientsManager.RegisterNewUserAsync(clientRegisterInfoDto);

            if (!clientRegisterStatus.isCreated)
                return BadRequest(clientRegisterStatus.RegisterMessage);

            return Ok( /*clientRegisterStatus.RegisterMessage*/
            );
        }

        [HttpPost]
        [Route("/Login")]
        public async Task<ActionResult<LoginStatusWithTokenDto>> LoginAsync(
            LoginInfoDto loginInfoDto
        )
        {
            LoginStatusWithTokenDto loginStatus = await _accountsManager.LoginAsync(loginInfoDto);

            if (!string.IsNullOrEmpty(loginStatus.RefreshToken))
                SetRefreshTokenInCookie(
                    loginStatus.RefreshToken,
                    loginStatus.RefreshTokenExpiration
                );

            return StatusCode((int)loginStatus.StatusCode, loginStatus);
        }

        #region Authentication & RefreshToken

        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime()
                //Secure = true,
                //IsEssential = true,
                //SameSite = SameSiteMode.None
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        [HttpGet("RenewTokens")]
        public async Task<IActionResult> RenewTokens()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            var result = await authManger.RenewTokens(refreshToken);

            if (!result.IsAuthenticated)
                return BadRequest(result);

            // Add new refreshToken to the cookies
            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }

        [HttpPost("revokeToken")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeRefreshTokenDto model)
        {
            // Recive the token from body OR cookies
            // must send an empty json.
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is required!");

            var result = await authManger.RevokeTokenAsync(token);

            if (!result)
                return BadRequest("Token is invalid!");

            return Ok();
        }

        #endregion
    }
}
