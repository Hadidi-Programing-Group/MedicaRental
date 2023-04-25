using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Admin;
using MedicaRental.BLL.Managers;
using MedicaRental.DAL.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MedicaRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAccountsManager _accountsManager;
        private readonly IClientsManager _clientsManager;
        private readonly UserManager<AppUser> _userManager;

        public UsersController(IAccountsManager accountsManager, IClientsManager clientsManager, UserManager<AppUser> userManager)
        {
            _accountsManager = accountsManager;
            _clientsManager = clientsManager;
            _userManager = userManager;
        }

        #region Raouf-Added

        [HttpGet]
        [Route("allClients")]
        public async Task<ActionResult<List<UserProfileInfoDto>>> GetAllClients()
        {
            var clients = await _clientsManager.GetAllClientsAsync();

            if (!clients.Any()) return NotFound();

            return Ok(clients);
        }


        [HttpGet]
        [Route("clientsNeedingApproval")]
        //[Authorize(Policy = ClaimRequirement.AdminPolicy)]
        public async Task<ActionResult<IEnumerable<UserProfileInfoDto>>> GetClientsNeedingApproval()
        {
            var clients = await _clientsManager.GetClientsNeedingApprovalAsync();

            if (!clients.Any())
            {
                return NotFound();
            }
            return Ok(clients);
        }

        [HttpGet]
        [Route("GetAllClientsApprovalInfo")]
        //[Authorize(Policy = ClaimRequirement.ClientPolicy)]
        public async Task<ActionResult<List<UserApprovalInfoDto>>> GetAllClientsApprovalInfo()
        {
            var approvalInfoList = await _clientsManager.GetAllClientsApprovalInfoAsync();

            if (!approvalInfoList.Any()) return NotFound();

            return Ok(approvalInfoList);
        }


        #endregion

        #region Old-endpoints

        [HttpGet]
        [Route("GetInfo")]
        //[Authorize(Policy = ClaimRequirement.ClientPolicy)]
        public async Task<ActionResult<UserProfileInfoDto>> GetUserInfo()
        {
            var userId = _userManager.GetUserId(User);

            if (userId is null) return Unauthorized();

            UserProfileInfoDto? userProfileInfo = await _clientsManager.GetClientInfoAsync(userId);

            if (userProfileInfo is null) return NotFound();

            return userProfileInfo;
        }

        [HttpPut]
        [Route("UpdateInfo")]
        [Authorize(Policy = ClaimRequirement.ClientPolicy)]
        public async Task<ActionResult<StatusDto>> UpdateUserInfo(UpdateProfileInfoDto updateProfileInfoDto)
        {
            var userId = _userManager.GetUserId(User);

            if (userId is null) return Unauthorized();

            StatusDto statusDto = await _clientsManager.UpdateClientInfoAsync(userId, updateProfileInfoDto);


            return statusDto;
        }

        [HttpGet]
        [Route("GetApprovalInfo")]
        //[Authorize(Policy = ClaimRequirement.ClientPolicy)]
        public async Task<ActionResult<UserApprovalInfoDto>> GetApprovalInfo()
        {
            var userId = _userManager.GetUserId(User);

            if (userId is null) return Unauthorized();

            UserApprovalInfoDto? userApprovalInfo = await _clientsManager.GetClientApprovalInfoAsync(userId);

            if (userApprovalInfo is null) return NotFound();

            return userApprovalInfo;
        }

        [HttpPut]
        [Route("UpdateApprovalInfo")]
        [Authorize(Policy = ClaimRequirement.ClientPolicy)]
        public async Task<ActionResult<StatusDto>> UpdateApprovalInfo(UpdateApprovalInfoDto updateProfileInfoDto)
        {
            var userId = _userManager.GetUserId(User);

            if (userId is null) return Unauthorized();

            StatusDto statusDto = await _clientsManager.UpdateApprovalInfoAsync(userId, updateProfileInfoDto);


            return statusDto;
        }

        [HttpPost]
        [Route("BlockUser")]
        public async Task<ActionResult<StatusDto>> BlockUserAsync(BlockUserInfoDto blockUserInfo)
        {
            var blockingStatus = await _accountsManager.BlockUserAsync(blockUserInfo);
            return StatusCode((int)blockingStatus.StatusCode, blockingStatus.StatusMessage);
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
        #endregion
    }
}
