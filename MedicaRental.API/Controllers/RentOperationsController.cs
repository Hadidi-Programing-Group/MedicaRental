using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedicaRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentOperationsController : ControllerBase
    {
        private readonly IRentOperationsManager _rentOperationsManager;

        public RentOperationsController(IRentOperationsManager rentOperationsManager)
        {
            _rentOperationsManager = rentOperationsManager;
        }

        [HttpGet("onrent/{userId}")]
        public async Task<ActionResult<PageDto<RentOperationDto>?>> GetOnRentItems(string userId, int page, string? orderBy)
        {
            var operations = await _rentOperationsManager.GetOnRentItemsAsync(userId, page, orderBy);

            if (operations is null) return BadRequest();

            return Ok(operations);
        }

        [HttpGet("onrent/history/{userId}")]
        public async Task<ActionResult<PageDto<RentOperationDto>?>> GetOnRentItemsHistory(string userId, int page, string? orderBy)
        {
            var operations = await _rentOperationsManager.GetOnRentItemsHistoryAsync(userId, page, orderBy);

            if (operations is null) return BadRequest();

            return Ok(operations);
        }

        [HttpGet("rented/{userId}")]
        public async Task<ActionResult<PageDto<RentOperationDto>?>> GetRentedItems(string userId, int page, string? orderBy)
        {
            var operations = await _rentOperationsManager.GetRentedItemsAsync(userId, page, orderBy);

            if (operations is null) return BadRequest();

            return Ok(operations);
        }

        [HttpGet("rented/history/{userId}")]
        public async Task<ActionResult<PageDto<RentOperationDto>?>> GetRentedItemsHistory(string userId, int page, string? orderBy)
        {
            var operations = await _rentOperationsManager.GetRentedItemsHistoryAsync(userId, page, orderBy);

            if (operations is null) return BadRequest();

            return Ok(operations);
        }
    }
}
