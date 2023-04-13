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
        private readonly UserManager<AppUser> _userManager;
        private readonly IClientsManager _clientsManager;
        private readonly IConfiguration _configuration;

        public AccountsController(
            UserManager<AppUser> userManager,
            IClientsManager clientsManager,
            IConfiguration configuration
            )
        {
            _userManager = userManager;
            _clientsManager = clientsManager;
            _configuration = configuration;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            return Ok("Auth Works");
        }

        [HttpPost]
        [Route("/Login")]
        public async Task<ActionResult<LoginTokenDto>> LoginAsync(LoginInfoDto loginInfoDto)
        {
            var user = await _userManager.FindByEmailAsync(loginInfoDto.Email);
            if (user is null)
                return NotFound();

            var isAuth = await _userManager.CheckPasswordAsync(user, loginInfoDto.Password);
            if (!isAuth)
                return Unauthorized();

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

            return new LoginTokenDto(tokenString, expiry);
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
