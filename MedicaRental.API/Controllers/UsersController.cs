using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedicaRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAccountsManager _accountsManager;

        public UsersController(IAccountsManager accountsManager)
        {
            _accountsManager = accountsManager;
        }
        [HttpPost]
        [Route("BlockUser")]
        public async Task<ActionResult<StatusDto>> BlockUserAsync(BlockUserInfoDto blockUserInfo)
        {
            var blockingStatus =  await _accountsManager.BlockUserAsync(blockUserInfo);
            return Ok(blockingStatus);
        }
    }
}
