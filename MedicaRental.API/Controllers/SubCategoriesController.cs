using MedicaRental.BLL.Dtos.SubCategory;
using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<List<SubCategoriesDto>>> GetAllSubCategory()
        {
            var subcategories = (await subCategoriesManager.GetAllAsync()).ToList();
            if (subcategories is null)
                return NotFound();
            return subcategories;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<SubCategoriesDto>> GetSubcategoryById(Guid id)
        {
            var subCategory = await subCategoriesManager.GetByIdAsync(id);
            if (subCategory is null) return NotFound();
            return subCategory;
        }

        [HttpPost]
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
        [Route("{id}")]
        public async Task<ActionResult> UpdateSubCategory(Guid id, UpdateSubCategoryDto SubCategory)
        {
            UpdateSubCategoryStatusDto UpdateStatus = await subCategoriesManager.UpdateSubCategory(id, SubCategory);
            if (!UpdateStatus.isUpdated)
                return BadRequest(UpdateStatus.StatusMessage);
            return CreatedAtAction(
                actionName: nameof(GetSubcategoryById),
                routeValues: new { Id = UpdateStatus.Id },
                value: new { Message = UpdateStatus.StatusMessage });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteSubCategory(Guid id)
        {
            DeleteSubCategoryStatusDto DeleteStatus = await subCategoriesManager.DeleteByIdAsync(id);
            if (DeleteStatus.isDeleted)
                   return NoContent();
            return BadRequest(DeleteStatus.StatusMessage);
        }


    }
}
