using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Admin;
using MedicaRental.BLL.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Permissions;

namespace MedicaRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Policy = ClaimRequirement.AdminPolicy)]

    public class AdminsController : ControllerBase
    {
        private readonly IAdminsManager _adminManger;

        public AdminsController(IAdminsManager adminManger)
        {
            _adminManger = adminManger;
        }


        [HttpPost]
        [Route("UpdateUserRole")]
        [Authorize(Policy = ClaimRequirement.AdminPolicy)]
        public async Task<ActionResult<UserRoleUpdateStatusDto>> UpdateUserRoleAsync(
            UserRoleUpdateDto userRoleUpdateDto
        )
        {
            var result = await _adminManger.UpdateUserRoleAsync(userRoleUpdateDto);

            // StatuesCode, and the full DTO object.
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost]
        [Route("UpdateReportStatus")]
        [Authorize(Policy = ClaimRequirement.ModeratorPolicy)]

        public async Task<ActionResult<ReportUpdateStatusDto>> UpdateReportStatusAsync(
            ReportUpdateDto reportUpdateDto
        )
        {
            var result = await _adminManger.UpdateReportStatus(reportUpdateDto);

            // StatuesCode, and the full DTO object.
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet]
        [Route("GetAllAdminMod")]
        [Authorize(Policy = ClaimRequirement.AdminPolicy)]

        public async Task<ActionResult<IEnumerable<RoleMangerUserInfoDto>>> GetAllAdminMod()
        {
            var result = await _adminManger.GetAllAdminMod();

            // StatuesCode, and the full DTO object.
            return Ok(result);
        }

        [HttpPost]
        [Route("DeleteAdminMod")]
        [Authorize(Policy = ClaimRequirement.AdminPolicy)]

        public async Task<ActionResult<StatusDto>> DeleteAdminMod(
         string id
     )
        {
            var result = await _adminManger.DeleteAdminMod(id);

            // StatuesCode, and the full DTO object.
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
