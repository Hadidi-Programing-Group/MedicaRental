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
        private readonly IReportActionManager _reportActionManager;
        private readonly UserManager<AppUser> _userManager;

        public UsersController(IAccountsManager accountsManager, 
            IClientsManager clientsManager, 
            IReportActionManager reportActionManager,
            UserManager<AppUser> userManager)
        {
            _accountsManager = accountsManager;
            _clientsManager = clientsManager;
            _reportActionManager = reportActionManager;
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
        [Route("GetAllClientsApprovalInfo")]
        //[Authorize(Policy = ClaimRequirement.ClientPolicy)]
        public async Task<ActionResult<List<UserApprovalInfoDto>>> GetAllClientsApprovalInfo()
        {
            var approvalInfoList = await _clientsManager.GetAllClientsApprovalInfoAsync();

            if (!approvalInfoList.Any()) return NotFound();

            return Ok(approvalInfoList);
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
        [Route("GetClientApprovalInfoWithId/{userId}")]
        //[Authorize(Policy = ClaimRequirement.ClientPolicy)]
        public async Task<ActionResult<UserApprovalInfoWithIdDto>> GetClientApprovalInfoWithId(string userId)
        {
            var approvalInfo = await _clientsManager.GetClientApprovalInfoWithIdAsync(userId);

            if (approvalInfo is null) return NotFound();

            return approvalInfo;
        }


        [HttpGet]
        [Route("GetInfo/{userId}")]
        //[Authorize(Policy = ClaimRequirement.ClientPolicy)]
        public async Task<ActionResult<UserProfileInfoDto>> GetUserInfo(string userId)
        {
            if (userId is null) return Unauthorized();

            UserProfileInfoDto? userProfileInfo = await _clientsManager.GetClientInfoAsync(userId);

            if (userProfileInfo is null) return NotFound();

            return userProfileInfo;
        }



        [HttpPut]
        [Route("UpdateApprovalInfo/{userId}")]
        //[Authorize(Policy = ClaimRequirement.ClientPolicy)]
        public async Task<ActionResult<StatusDto>> UpdateApprovalInfo(string userId, UpdateApprovalInfoRejectDto updateProfileInfoDto)
        {
            //var userId = _userManager.GetUserId(User);

            if (userId is null) return Unauthorized();

            StatusDto statusDto = await _clientsManager.UpdateApprovalInfoRejectAsync(userId, updateProfileInfoDto);


            return statusDto;
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
        [Authorize(Policy = ClaimRequirement.AdminPolicy)]
        public async Task<ActionResult<StatusDto>> BlockUserAsync(BlockUserInfoDto blockUserInfo)
        {
            var userId =  _userManager.GetUserId(User);
            if (userId is null)
                return Unauthorized();
            var blockingStatus = await _accountsManager.BlockUserAsync(blockUserInfo);

            if (blockUserInfo.ReportId is null || blockingStatus.StatusCode != System.Net.HttpStatusCode.OK)
                return StatusCode((int)blockingStatus.StatusCode, blockingStatus);

            var insertReportActionDto = new InserReportActionDto(blockingStatus.StatusMessage, blockUserInfo.ReportId.Value, userId);
            var addingReportAction = await _reportActionManager.AddReportAction(insertReportActionDto);

            return StatusCode((int)addingReportAction.StatusCode, blockingStatus);
        }

        [HttpPost]
        [Route("UnBlockUser")]
        [Authorize(Policy = ClaimRequirement.AdminPolicy)]
        public async Task<ActionResult<StatusDto>> UnBlockUserAsync(string Email)
        {
            StatusDto blockingStatus = await _accountsManager.UnBlockUserAsync(Email);
            return StatusCode((int)blockingStatus.StatusCode, new { blockingStatus.StatusMessage });
        }

        [HttpPost]
        [Route("ApproveUser")]
        public async Task<ActionResult<StatusDto>> ApproveUserAsync(string Email)
        {
            StatusDto blockingStatus = await _clientsManager.ApproveUserAsync(Email);
            return StatusCode((int)blockingStatus.StatusCode, new { blockingStatus.StatusMessage });
        } 
        #endregion
    }
}
