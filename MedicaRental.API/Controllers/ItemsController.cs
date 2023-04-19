using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Managers;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MedicaRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsManager _itemsManager;
        private readonly UserManager<AppUser> _userManager;

        public ItemsController(IItemsManager itemsManager, UserManager<AppUser> userManager)
        {
            _itemsManager = itemsManager;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<PageDto<HomeItemDto>>> GetAllItems(int page, string? orderBy)
        {
            var items = await _itemsManager.GetAllItemsAsync(page, orderBy);

            if (items is null) return BadRequest();

            return Ok(items);
        }

        [HttpGet("seller/{sellerId}")]
        public async Task<ActionResult<PageDto<HomeItemDto>>> GetItemsBySeller(string sellerId, int page, string? orderBy)
        {
            var items = await _itemsManager.GetItemsBySellerAsync(sellerId, page, orderBy);

            if (items is null) return BadRequest();

            return Ok(items);
        }

        [HttpGet("search")]
        public async Task<ActionResult<PageDto<HomeItemDto>>> GetItemsBySearch(string searchText, int page, string? orderBy)
        {
            var items = await _itemsManager.GetItemsBySearchAsync(searchText, page, orderBy);

            if (items is null) return BadRequest();

            return Ok(items);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<PageDto<HomeItemDto>>> GetItemsByCategory(Guid categoryId, int page, string? orderBy)
        {
            var items = await _itemsManager.GetItemsByCategoryAsync(categoryId, page, orderBy);

            if (items is null) return BadRequest();

            return Ok(items);
        }

        [HttpGet("subcategory/{subcategoryId}")]
        public async Task<ActionResult<PageDto<HomeItemDto>>> GetItemsBySubCategory(Guid subcategoryId, int page, string? orderBy)
        {
            var items = await _itemsManager.GetItemsBySubCategoryAsync(subcategoryId, page, orderBy);

            if (items is null) return BadRequest();

            return Ok(items);
        }

        [HttpGet("categories")]
        public async Task<ActionResult<PageDto<HomeItemDto>>> GetItemsByCategories([FromQuery] IEnumerable<Guid> categoryIds, int page, string? orderBy)
        {
            var items = await _itemsManager.GetItemsByCategoriesAsync(categoryIds, page, orderBy);

            if (items is null) return BadRequest();

            return Ok(items);
        }

        [HttpGet("subcategories")]
        public async Task<ActionResult<PageDto<HomeItemDto>>> GetItemsBySubCategories([FromQuery] IEnumerable<Guid> subcategoryIds, int page, string? orderBy)
        {
            var items = await _itemsManager.GetItemsBySubCategoriesAsync(subcategoryIds, page, orderBy);

            if (items is null) return BadRequest();

            return Ok(items);
        }

        [HttpGet("forseller")]
        public async Task<ActionResult<PageDto<SellerItemDto>>> GetAllItemsForSeller(int page, string? orderBy)
        {
            var items = await _itemsManager.GetAllItemsForSellerAsync(page, orderBy);

            if (items is null) return BadRequest();

            return Ok(items);
        }

        [HttpGet("forrenter")]
        public async Task<ActionResult<PageDto<RenterItemDto>>> GetAllItemsForRenter(int page, string? orderBy)
        {
            var items = await _itemsManager.GetAllItemsForRenterAsync(page, orderBy);

            if (items is null) return BadRequest();

            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HomeItemDto>> FindItem(Guid id)
        {
            var item = await _itemsManager.FindItemAsync(id);

            if (item is null) return BadRequest();

            return Ok(item);
        }
        
        [HttpGet("forseller/{id}")]
        public async Task<ActionResult<SellerItemDto>> FindItemForSeller(Guid id)
        {
            var item = await _itemsManager.FindItemForSellerAsync(id);

            if (item is null) return BadRequest();

            return Ok(item);
        }
        
        [HttpGet("forrenter/{id}")]
        public async Task<ActionResult<RenterItemDto>> FindItemForRenter(Guid id)
        {
            var item = await _itemsManager.FindItemForRenterAsync(id);

            if (item is null) return BadRequest();

            return Ok(item);
        }
        
        [HttpPost("one")]
        public async Task<ActionResult<StatusDto>> AddItem(AddItemDto item)
        {
            return await _itemsManager.AddItemAsync(item);
        }
        
        [HttpPost("many")]
        public async Task<ActionResult<StatusDto>> AddItems(IEnumerable<AddItemDto> items)
        {
            return await _itemsManager.AddItemsAsync(items);
        }
        
        [HttpPut("one")]
        public async Task<ActionResult<StatusDto>> UpdateItem(UpdateItemDto item)
        {
            return await _itemsManager.UpdateItem(item);
        }
        
        [HttpPut("many")]
        public async Task<ActionResult<StatusDto>> UpdateItems(IEnumerable<UpdateItemDto> items)
        {
            return await _itemsManager.UpdateItems(items);
        }
        
        [HttpDelete("one")]
        public async Task<ActionResult<StatusDto>> DeleteItem(Guid id)
        {
            return await _itemsManager.DeleteItem(id);
        }
        
        [HttpDelete("many")]
        public async Task<ActionResult<StatusDto>> DeleteItems(IEnumerable<Guid> ids)
        {
            return await _itemsManager.DeleteItems(ids);
        }
        
        [HttpPut("unlist/{itemId}")]
        public async Task<ActionResult<StatusDto>> UnListItem(Guid itemId)
        {
            return await _itemsManager.UnListItem(itemId);
        }

        [HttpPut("relist/{itemId}")]
        public async Task<ActionResult<StatusDto>> ReListItem(Guid itemId)
        {
            return await _itemsManager.ReListItem(itemId);
        }

        [HttpGet("listed")]
        public async Task<ActionResult<PageDto<ListItemDto>>?> GetListedItems(int page, string? orderBy)
        {
            var userId = "a193d7d4-6840-42f5-bba2-5da6e8ff6a24";//_userManager.GetUserId(User);

            if (userId == null) return StatusCode(500);
            
            var items = await _itemsManager.GetListedItemsAsync(userId, page, orderBy);

            if (items is null) return BadRequest();

            return Ok(items);
        }

        [HttpGet("unlisted")]
        public async Task<ActionResult<PageDto<ListItemDto>>?> GetUnListedItems(int page, string? orderBy)
        {
            var userId = "a193d7d4-6840-42f5-bba2-5da6e8ff6a24";//_userManager.GetUserId(User);

            if (userId == null) return StatusCode(500);

            var items = await _itemsManager.GetUnListedItemsAsync(userId, page, orderBy);

            if (items is null) return BadRequest();

            return Ok(items);
        }
    }
}
