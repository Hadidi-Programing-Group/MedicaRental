using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Managers;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MedicaRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IClientsManager _clientsManager;

        public AccountsController(
            UserManager<AppUser> userManager,
            IClientsManager clientsManager)
        {
            _userManager = userManager;
            _clientsManager = clientsManager;
        }

        [HttpPost]
        [Route("/Login")]
        public ActionResult<LoginTokenDto> Login(LoginInfoDto loginInfoDto)
        {
            return Ok();
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
