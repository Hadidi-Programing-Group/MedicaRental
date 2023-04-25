using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Report;
using MedicaRental.BLL.Dtos.Review;
using MedicaRental.BLL.Dtos.SubCategory;
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
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewsManager _reviewssManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IItemsManager itemsManager;
        public ReviewsController(IReviewsManager reviewsManager,IItemsManager _itemsManager, UserManager<AppUser> userManager)
        {
            _reviewssManager = reviewsManager;
            _userManager = userManager;
            itemsManager = _itemsManager;
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
        public async Task<ActionResult> InsertReview(InsertReviewForClientDto reviewDto)
        {
            var userId = _userManager.GetUserId(User);
            HomeItemDto? item = await itemsManager.FindItemAsync(reviewDto.ItemId);
            if(item is null) return NotFound();

            InsertReviewDto NewReview = new(
                Id:Guid.Empty,
                Rating: reviewDto.Rating,
                IsDeleted: reviewDto.IsDeleted,
                ClientReview: reviewDto.ClientReview,
                ClientId: userId,
                SellerId: item.SellerId,
                ItemId: reviewDto.ItemId);

            InsertReviewStatusDto InsertStatus = await _reviewssManager.InsertReview(NewReview);
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
