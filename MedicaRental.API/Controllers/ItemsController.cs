using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Managers;
using MedicaRental.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedicaRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsManager _itemsManager;

        public ItemsController(IItemsManager itemsManager)
        {
            _itemsManager = itemsManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HomeItemDto>>> GetAllItems(string? orderBy)
        {
            var items = await _itemsManager.GetAllItemsAsync(orderBy);

            if (items is null) return BadRequest();

            return Ok(items);
        }

        [HttpGet("seller/{sellerId}")]
        public async Task<ActionResult<IEnumerable<HomeItemDto>>> GetItemsBySeller(string sellerId, string? orderBy)
        {
            var items = await _itemsManager.GetItemsBySellerAsync(sellerId, orderBy);

            if (items is null) return BadRequest();

            return Ok(items);
        }

        [HttpGet("search/{searchText}")]
        public async Task<ActionResult<IEnumerable<HomeItemDto>>> GetItemsBySearch(string searchText, string? orderBy)
        {
            var items = await _itemsManager.GetItemsBySearchAsync(searchText, orderBy);

            if (items is null) return BadRequest();

            return Ok(items);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<HomeItemDto>>> GetItemsByCategory(Guid categoryId, string? orderBy)
        {
            var items = await _itemsManager.GetItemsByCategoryAsync(categoryId, orderBy);

            if (items is null) return BadRequest();

            return Ok(items);
        }

        [HttpGet("subcategory/{subcategoryId}")]
        public async Task<ActionResult<IEnumerable<HomeItemDto>>> GetItemsBySubCategory(Guid subcategoryId, string? orderBy)
        {
            var items = await _itemsManager.GetItemsBySubCategoryAsync(subcategoryId, orderBy);

            if (items is null) return BadRequest();

            return Ok(items);
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<HomeItemDto>>> GetItemsByCategories([FromQuery] IEnumerable<Guid> categoryIds, string? orderBy)
        {
            var items = await _itemsManager.GetItemsByCategoriesAsync(categoryIds, orderBy);

            if (items is null) return BadRequest();

            return Ok(items);
        }

        [HttpGet("subcategories")]
        public async Task<ActionResult<IEnumerable<HomeItemDto>>> GetItemsBySubCategories([FromQuery] IEnumerable<Guid> subcategoryIds, string? orderBy)
        {
            var items = await _itemsManager.GetItemsBySubCategoriesAsync(subcategoryIds, orderBy);

            if (items is null) return BadRequest();

            return Ok(items);
        }

        [HttpGet("forseller")]
        public async Task<ActionResult<IEnumerable<SellerItemDto>>> GetAllItemsForSeller(string? orderBy)
        {
            var items = await _itemsManager.GetAllItemsForSellerAsync(orderBy);

            if (items is null) return BadRequest();

            return Ok(items);
        }

        [HttpGet("forrenter")]
        public async Task<ActionResult<IEnumerable<RenterItemDto>>> GetAllItemsForRenter(string? orderBy)
        {
            var items = await _itemsManager.GetAllItemsForRenterAsync(orderBy);

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
        
        [HttpPut("unlist/{id}")]
        public async Task<ActionResult<StatusDto>> UnListItem(Guid id)
        {
            return await _itemsManager.UnListItem(id);
        }
    }
}
