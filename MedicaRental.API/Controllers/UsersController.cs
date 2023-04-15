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
        private readonly IClientsManager _clientsManager;

        public UsersController(IAccountsManager accountsManager, IClientsManager clientsManager)
        {
            _accountsManager = accountsManager;
            _clientsManager = clientsManager;
        }

        [HttpPost]
        [Route("BlockUser")]
        public async Task<ActionResult<StatusDto>> BlockUserAsync(BlockUserInfoDto blockUserInfo)
        {
            var blockingStatus =  await _accountsManager.BlockUserAsync(blockUserInfo);
            return StatusCode((int) blockingStatus.StatusCode, blockingStatus.StatusMessage);
        }

        [HttpPost]
        [Route("UnBlockUser")]
        public async Task<ActionResult<StatusDto>> UnBlockUserAsync(string Email)
        {
            StatusDto blockingStatus = await _accountsManager.UnBlockUserAsync(Email);
            return StatusCode((int)blockingStatus.StatusCode, blockingStatus.StatusMessage);
        }

        [HttpPost]
        [Route("ApproveUser")]
        public async Task<ActionResult<StatusDto>> ApproveUserAsync(string Email)
        {
            StatusDto blockingStatus = await _clientsManager.ApproveUserAsync(Email);
            return StatusCode((int)blockingStatus.StatusCode, blockingStatus.StatusMessage);
        }
    }
}
