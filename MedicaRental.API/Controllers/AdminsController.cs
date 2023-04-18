using MedicaRental.BLL.Dtos.Admin;
using MedicaRental.BLL.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedicaRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly IAdminsManager _adminManger;

        public AdminsController(IAdminsManager adminManger)
        {
            _adminManger = adminManger;
        }

        [HttpPost]
        [Route("UpdateUserRole")]
        //[Authorize(Policy = ClaimRequirement.AdminPolicy)]
        public async Task<ActionResult<UserRoleUpdateStatusDto>> UpdateUserRoleAsync(
            UserRoleUpdateDto userRoleUpdateDto
        )
        {
            var result = await _adminManger.UpdateUserRoleAsync(userRoleUpdateDto);

            // StatuesCode, and the full DTO object. 
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
