using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Report;
using MedicaRental.BLL.Dtos.SubCategory;
using MedicaRental.BLL.Managers;
using MedicaRental.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedicaRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewsManager _reviewssManager;
        public ReviewsController(IReviewsManager reviewsManager)
        {
            _reviewssManager = reviewsManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReviewDto>>> GetAllReviews()
        {
            List<ReviewDto> reviews = (await _reviewssManager.GetAllAsync()).ToList();
            if (reviews == null)
                return NotFound();

            return reviews;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ReviewDto>> GetReviewById(Guid id)
        {
            var review = await _reviewssManager.GetByIdAsync(id);
            if (review is null) return NotFound();
            return review;
        }

        [HttpPost]
        public async Task<ActionResult> InsertReview(InsertReviewDto reviewDto)
        {
            InsertReviewStatusDto InsertStatus = await _reviewssManager.InsertReview(reviewDto);
            if (!InsertStatus.isCreated)
                return BadRequest(InsertStatus.StatusMessage);
            return CreatedAtAction(
                actionName: nameof(GetReviewById),
                routeValues: new { Id = InsertStatus.Id },
                value: new { Message = InsertStatus.StatusMessage });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteReview(Guid id)
        {
            DeleteReviewStatusDto DeleteStatus = await _reviewssManager.DeleteByIdAsync(id);
            if (DeleteStatus.isDeleted)
                  return NoContent();
            return BadRequest(DeleteStatus.StatusMessage);
        }
    }
}
