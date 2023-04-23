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
    [Authorize(Policy = ClaimRequirement.ClientPolicy)]
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
        public async Task<ActionResult<PageDto<RentOperationDto>?>> GetOnRentItems(int page, string? orderBy, string? searchText)
        {
            var userId = _userManager.GetUserId(User);
            
            var operations = await _rentOperationsManager.GetOnRentItemsAsync(userId, page, orderBy, searchText);

            if (operations is null) return BadRequest();

            return Ok(operations);
        }

        [HttpGet("onrent/history")]
        public async Task<ActionResult<PageDto<RentOperationDto>?>> GetOnRentItemsHistory(int page, string? orderBy, string? searchText)
        {
            var userId = _userManager.GetUserId(User);
           
            var operations = await _rentOperationsManager.GetOnRentItemsHistoryAsync(userId, page, orderBy, searchText);

            if (operations is null) return BadRequest();

            return Ok(operations);
        }

        [HttpGet("rented")]
        public async Task<ActionResult<PageDto<RentOperationDto>?>> GetRentedItems(int page, string? orderBy, string? searchText)
        {
            var userId = _userManager.GetUserId(User);

            var operations = await _rentOperationsManager.GetRentedItemsAsync(userId, page, orderBy, searchText);

            if (operations is null) return BadRequest();

            return Ok(operations);
        }

        [HttpGet("rented/history")]
        public async Task<ActionResult<PageDto<RentOperationDto>?>> GetRentedItemsHistory(int page, string? orderBy, string? searchText)
        {
            var userId = _userManager.GetUserId(User);

            var operations = await _rentOperationsManager.GetRentedItemsHistoryAsync(userId, page, orderBy, searchText);

            if (operations is null) return BadRequest();

            return Ok(operations);
        }

        [HttpGet]
        [Route("isrented/{itemId}")]
        public async Task<ActionResult<ItemHasBeenRentedToUserDto>> GetIsItemRentedStatus (Guid itemId)
        {
            var userId = _userManager.GetUserId(User);
            var IsReturned = await _rentOperationsManager.GetRentingStatus(userId, itemId);
            return IsReturned;
        }
    }
}
