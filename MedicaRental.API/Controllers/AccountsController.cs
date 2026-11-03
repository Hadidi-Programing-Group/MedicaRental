using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Managers;
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

        public AccountsController(
            IClientsManager clientsManager,
            IAccountsManager accountsManager
            )
        {
            _clientsManager = clientsManager;
            _accountsManager = accountsManager;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            return Ok("Auth Works");
        }

        [HttpPost]
        [Route("/Login")]
        public async Task<ActionResult<LoginStatusWithTokenDto>> LoginAsync(LoginInfoDto loginInfoDto)
        {
            LoginStatusWithTokenDto loginStatus = await _accountsManager.LoginAsync(loginInfoDto);

            return StatusCode((int)loginStatus.StatusCode, loginStatus);
        }

        [HttpPost]
        [Route("/Register")]
        public async Task<ActionResult> RegisterAsync(ClientRegisterInfoDto clientRegisterInfoDto)
        {
            ClientRegisterStatusDto clientRegisterStatus = await _clientsManager.RegisterNewUserAsync(clientRegisterInfoDto);

            if (!clientRegisterStatus.isCreated)
                return BadRequest(clientRegisterStatus.RegisterMessage);

            return Ok(clientRegisterStatus.RegisterMessage);
        }
    }
}
