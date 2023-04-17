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
        public async Task<ActionResult<IEnumerable<RentOperationDto>?>> GetOnRentItems(string userId, string? orderBy)
        {
            var operations = await _rentOperationsManager.GetOnRentItemsAsync(userId, orderBy);

            if (operations is null) return BadRequest();

            return Ok(operations);
        }

        [HttpGet("onrent/history/{userId}")]
        public async Task<ActionResult<IEnumerable<RentOperationDto>?>> GetOnRentItemsHistory(string userId, string? orderBy)
        {
            var operations = await _rentOperationsManager.GetOnRentItemsHistoryAsync(userId, orderBy);

            if (operations is null) return BadRequest();

            return Ok(operations);
        }

        [HttpGet("rented/{userId}")]
        public async Task<ActionResult<IEnumerable<RentOperationDto>?>> GetRentedItems(string userId, string? orderBy)
        {
            var operations = await _rentOperationsManager.GetRentedItemsAsync(userId, orderBy);

            if (operations is null) return BadRequest();

            return Ok(operations);
        }

        [HttpGet("rented/history/{userId}")]
        public async Task<ActionResult<IEnumerable<RentOperationDto>?>> GetRentedItemsHistory(string userId, string? orderBy)
        {
            var operations = await _rentOperationsManager.GetRentedItemsHistoryAsync(userId, orderBy);

            if (operations is null) return BadRequest();

            return Ok(operations);
        }
    }
}
