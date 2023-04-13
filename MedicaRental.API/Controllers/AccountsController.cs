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
        public async Task<ActionResult> RegisterAsync(RegisterInfoDto registerInfoDto)
        {
            RegisterStatusDto registerStatus = await _clientsManager.RegisterNewUserAsync(registerInfoDto);
            if (!registerStatus.isCreated)
                return BadRequest(registerStatus.RegisterMessage);

            return Ok(registerStatus.RegisterMessage);
        }
    }
}
