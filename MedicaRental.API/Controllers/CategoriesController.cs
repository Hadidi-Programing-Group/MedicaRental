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

        [HttpGet]
        public async Task<ActionResult<List<CategoryDto>>> GetAll()
        {
            List<CategoryDto> Categories = (await _categoriesManager.GetAllAsyc()).ToList();
            if (Categories is null)
                return NotFound();

            return Categories;
        }

        [HttpGet]
        [Route("{Id}")]
        public async Task<ActionResult<CategoryWithSubCategoriesDto>> GetAllWithSubCategories(int Id)
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
        [Route("{Id}")]
        public async Task<ActionResult> UpdateCategory(int Id, UpdateCategoryDto updateCategoryDto)
        {
            UpdateCategoryStatusDto updateCategoryStatus = await _categoriesManager.UpdateNewCategory(Id, updateCategoryDto);

            if (!updateCategoryStatus.isUpdated)
                return BadRequest(updateCategoryStatus.StatusMessage);

            return CreatedAtAction(
                actionName: nameof(GetAllWithSubCategories),
                routeValues: new { Id = updateCategoryStatus.Id },
                value: new { Message = updateCategoryStatus.StatusMessage }
                );
        }

        [HttpDelete]
        [Route("{Id}")]
        public async Task<ActionResult> DeleteCategoryAsync(int Id)
        {
            DeleteCategoryStatusDto deleteCategoryStatus = await _categoriesManager.DeleteByIdAsync(Id);

            if (!deleteCategoryStatus.isDeleted)
                return BadRequest(deleteCategoryStatus.StatusMessage);

            return NoContent();
        }

    }
}
