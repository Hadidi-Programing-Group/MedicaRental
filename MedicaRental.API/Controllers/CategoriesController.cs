using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedicaRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesManager _categoriesManager;

        public CategoriesController(ICategoriesManager categoriesManager)
        {
            _categoriesManager = categoriesManager;
        }

        [HttpGet("withsub")]
        public async Task<ActionResult<IEnumerable<CategoryWithSubCategoriesDto>>> GetAllWithSubCategories()
        {
            IEnumerable<CategoryWithSubCategoriesDto> Categories = await _categoriesManager.GetAllWithSubCategoriesAsync();

            if (Categories is null) return BadRequest();

            return Ok(Categories);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PageDto<CategoryDto>>> GetAll(int page, string? searchText)
        {
            var categories = await _categoriesManager.GetAllAsync(page, searchText);

            if (categories is null) return BadRequest();

            return Ok(categories);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
        {
            var categories = await _categoriesManager.GetAllAsync();

            if (categories is null) return BadRequest();

            return Ok(categories);
        }


        [HttpGet]
        [Route("{Id}")]
        public async Task<ActionResult<CategoryWithSubCategoriesDto>> GetCategoryWithSubCategories(Guid Id)
        {
            CategoryWithSubCategoriesDto? categoryWithSubCategories = await _categoriesManager.GetCategoryWithSubCategories(Id);
            if (categoryWithSubCategories is null) 
                return NotFound();

            return categoryWithSubCategories;
        }

        [HttpPost]
        public async Task<ActionResult> InsertCategory(InsertCategoryDto insertCategoryDto)
        {
            InsertCategoryStatusDto insertCategoryStatus = await _categoriesManager.InsertNewCategory(insertCategoryDto);

            if (!insertCategoryStatus.isCreated)
                return BadRequest(insertCategoryStatus.StatusMessage);

            return CreatedAtAction(
                actionName: nameof(GetAllWithSubCategories),
                routeValues: new { Id = insertCategoryStatus.Id },
                value: new { Message = insertCategoryStatus.StatusMessage }
                );
        }

        [HttpPut]
        public async Task<ActionResult> UpdateCategory(UpdateCategoryDto updateCategoryDto)
        {
            UpdateCategoryStatusDto updateCategoryStatus = await _categoriesManager.UpdateNewCategory(updateCategoryDto);

            if (!updateCategoryStatus.isUpdated)
                return BadRequest(updateCategoryStatus.StatusMessage);

            return NoContent();
        }

        [HttpDelete]
        [Route("{Id}")]
        public async Task<ActionResult> DeleteCategoryAsync(Guid Id)
        {
            DeleteCategoryStatusDto deleteCategoryStatus = await _categoriesManager.DeleteByIdAsync(Id);

            if (!deleteCategoryStatus.isDeleted)
                return BadRequest(deleteCategoryStatus.StatusMessage);

            return NoContent();
        }

    }
}
