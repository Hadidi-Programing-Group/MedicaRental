using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Brand;
using MedicaRental.BLL.Managers;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MedicaRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandsManager _brandssManager;

        public BrandsController(IBrandsManager brandssManager)
        {
            _brandssManager = brandssManager;
        }


        [HttpGet]
        public async Task<ActionResult<List<BrandDto>>> GetAll()
        {
            List<BrandDto> Brands = (await _brandssManager.GetAllAsyc()).ToList();

            if (Brands is null) return BadRequest();

            return Brands;
        }

        [HttpGet]
        [Route("{Id}")]
        public async Task<ActionResult<BrandDto>> GetById(Guid Id)
        {
            BrandDto? brandDto = await _brandssManager.GetBrandbyId(Id);
            if (brandDto is null)
                return NotFound();

            return brandDto;
        }



        [HttpPost]
        public async Task<ActionResult> InsertBrand(InsertBrandDto insertBrandDto)
        {
            InsertBrandStatusDto insertBrandStatus = await _brandssManager.InsertNewBrand(insertBrandDto);

            if (!insertBrandStatus.isCreated)
                return BadRequest(insertBrandStatus.StatusMessage);

            return CreatedAtAction(
                actionName: nameof(GetById),
                routeValues: new { Id = insertBrandStatus.Id },
                value: new { Message = insertBrandStatus.StatusMessage }
                );
        }


        [HttpPut]
        [Route("{Id}")]
        public async Task<ActionResult> UpdateBrand(Guid Id, UpdateBrandDto updateBrandDto)
        {
            UpdateBrandStatusDto updateBrandStatus = await _brandssManager.UpdateNewBrand(Id, updateBrandDto);

            if (!updateBrandStatus.isUpdated)
                return BadRequest(updateBrandStatus.StatusMessage);

            return CreatedAtAction(
                actionName: nameof(GetById),
                routeValues: new { Id = updateBrandStatus.Id },
                value: new { Message = updateBrandStatus.StatusMessage }
                );
        }

        [HttpDelete]
        [Route("{Id}")]
        public async Task<ActionResult> DeleteBrandAsync(Guid Id)
        {
            DeleteBrandStatusDto deleteBrandStatus = await _brandssManager.DeleteByIdAsync(Id);

            if (!deleteBrandStatus.isDeleted)
                return BadRequest(deleteBrandStatus.StatusMessage);

            return NoContent();
        }
    }
}
