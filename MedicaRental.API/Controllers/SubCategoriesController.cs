using MedicaRental.BLL.Dtos.SubCategory;
using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MedicaRental.BLL.Dtos.Admin;

namespace MedicaRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoriesController : ControllerBase
    {
        private readonly ISubCategoriesManager subCategoriesManager;

        public SubCategoriesController(ISubCategoriesManager _subCategoriesManager)
        {
            subCategoriesManager = _subCategoriesManager;
        }

        [HttpGet]
        public async Task<ActionResult<PageDto<SubCategoryWithCategoryDto>>> GetAllWithCategory(int page, string? searchText)
        {
            var subcategories = (await subCategoriesManager.GetAllWithCategoryAsync(page, searchText));
            if (subcategories is null)
                return NotFound();
            return subcategories;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<SubCategoryWithCategoryDto>> GetSubcategoryById(Guid id)
        {
            var subCategory = await subCategoriesManager.GetByIdAsync(id);
            if (subCategory is null) return NotFound();
            return subCategory;
        }

        [HttpPost]
        [Authorize(Policy = ClaimRequirement.ModeratorPolicy)]
        public async Task<ActionResult> InsertSubCategory(InsertSubCategoryDto SubCat)
        {
            InsertSubCategoryStatusDto InsertStatus = await subCategoriesManager.InsertSubCategory(SubCat);
            if (!InsertStatus.isCreated)
                return BadRequest(InsertStatus.StatusMessage);
            return CreatedAtAction(
                actionName: nameof(GetSubcategoryById),
                routeValues: new { Id = InsertStatus.Id },
                value: new { Message = InsertStatus.StatusMessage });
        }

        [HttpPut]
        [Authorize(Policy = ClaimRequirement.ModeratorPolicy)]
        public async Task<ActionResult> UpdateSubCategory(UpdateSubCategoryDto SubCategory)
        {
            UpdateSubCategoryStatusDto UpdateStatus = await subCategoriesManager.UpdateSubCategory(SubCategory);
            if (!UpdateStatus.isUpdated)
                return BadRequest(UpdateStatus.StatusMessage);
            return CreatedAtAction(
                actionName: nameof(GetSubcategoryById),
                routeValues: new { Id = UpdateStatus.Id },
                value: new { Message = UpdateStatus.StatusMessage });
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Policy = ClaimRequirement.ModeratorPolicy)]
        public async Task<ActionResult> DeleteSubCategory(Guid id)
        {
            DeleteSubCategoryStatusDto DeleteStatus = await subCategoriesManager.DeleteByIdAsync(id);
            if (DeleteStatus.isDeleted)
                   return NoContent();
            return BadRequest(DeleteStatus.StatusMessage);
        }
    }
}
