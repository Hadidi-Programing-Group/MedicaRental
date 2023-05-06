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

    public class RentOperationsController : ControllerBase
    {
        private readonly IRentOperationsManager _rentOperationsManager;
        private readonly UserManager<AppUser> _userManager;

        public RentOperationsController(IRentOperationsManager rentOperationsManager, UserManager<AppUser> userManager)
        {
            _rentOperationsManager = rentOperationsManager;
            _userManager = userManager;
        }

        [HttpGet("onrent")]
        [Authorize(Policy = ClaimRequirement.ClientPolicy)]
        public async Task<ActionResult<PageDto<RentOperationDto>?>> GetOnRentItems(int page, string? orderBy, string? searchText)
        {
            var userId = _userManager.GetUserId(User);
            
            var operations = await _rentOperationsManager.GetOnRentItemsAsync(userId, page, orderBy, searchText);

            if (operations is null) return BadRequest();

            return Ok(operations);
        }

        [HttpGet("onrent/history")]
        [Authorize(Policy = ClaimRequirement.ClientPolicy)]
        public async Task<ActionResult<PageDto<RentOperationDto>?>> GetOnRentItemsHistory(int page, string? orderBy, string? searchText)
        {
            var userId = _userManager.GetUserId(User);
           
            var operations = await _rentOperationsManager.GetOnRentItemsHistoryAsync(userId, page, orderBy, searchText);

            if (operations is null) return BadRequest();

            return Ok(operations);
        }

        [HttpGet("rented")]
        [Authorize(Policy = ClaimRequirement.ClientPolicy)]
        public async Task<ActionResult<PageDto<RentOperationDto>?>> GetRentedItems(int page, string? orderBy, string? searchText)
        {
            var userId = _userManager.GetUserId(User);

            var operations = await _rentOperationsManager.GetRentedItemsAsync(userId, page, orderBy, searchText);

            if (operations is null) return BadRequest();

            return Ok(operations);
        }

        [HttpGet("rented/history")]
        [Authorize(Policy = ClaimRequirement.ClientPolicy)]
        public async Task<ActionResult<PageDto<RentOperationDto>?>> GetRentedItemsHistory(int page, string? orderBy, string? searchText)
        {
            var userId = _userManager.GetUserId(User);

            var operations = await _rentOperationsManager.GetRentedItemsHistoryAsync(userId, page, orderBy, searchText);

            if (operations is null) return BadRequest();

            return Ok(operations);
        }

        [HttpGet]
        [Route("isrented/{itemId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ItemHasBeenRentedToUserDto>> GetIsItemRentedStatus (Guid itemId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId is null)
                return new ItemHasBeenRentedToUserDto(false);
            var IsReturned = await _rentOperationsManager.GetRentingStatus(userId, itemId);
            return IsReturned;
        }


        [HttpPost]  
        [Authorize(Policy = ClaimRequirement.ModeratorPolicy)]
        public async Task<ActionResult<ItemHasBeenRentedToUserDto>> InsertRentOperation(InsertRentOperationDto rentOperationDto)
        {
            var id = await _rentOperationsManager.AddRentOperation(rentOperationDto);

            if(id  is null) return BadRequest();

            return NoContent();
        }

        [HttpGet("GetRentedItems")]
        [Authorize(Policy = ClaimRequirement.ModeratorPolicy)]
        public async Task<IActionResult> GetRentedItems()
        {
            var data = await _rentOperationsManager.GetRentedItemsAsync();
            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        [HttpPut("acceptReturn/{rentOperationId}")]
        [Authorize(Policy = ClaimRequirement.ModeratorPolicy)]
        public async Task<IActionResult> AcceptReturn(Guid rentOperationId)
        {
            var result = await _rentOperationsManager.AcceptReturnAsync(rentOperationId);
            //return StatusCode((int)result.StatusCode, result.StatusMessage);
            return Ok(new { message = result.StatusMessage });
        }
    }
}
