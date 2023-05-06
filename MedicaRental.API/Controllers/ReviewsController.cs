using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Admin;
using MedicaRental.BLL.Dtos.Report;
using MedicaRental.BLL.Dtos.Review;
using MedicaRental.BLL.Dtos.SubCategory;
using MedicaRental.BLL.Managers;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicaRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewsManager _reviewssManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IItemsManager itemsManager;
        private readonly IReportActionManager _reportActionManager;

        public ReviewsController(IReviewsManager reviewsManager,
            IItemsManager _itemsManager,
            IReportActionManager reportActionManager,
            UserManager<AppUser> userManager)
        {
            _reviewssManager = reviewsManager;
            _userManager = userManager;
            itemsManager = _itemsManager;
            _reportActionManager = reportActionManager;
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
        [Authorize(Policy = ClaimRequirement.ClientPolicy)]
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

        [HttpPost]
        [Route("DeleteReview")]
        [Authorize]
        public async Task<ActionResult> DeleteReview(DeleteReviewRequestDto deleteReviewRequest)
        {
            var currentUserId = _userManager.GetUserId(User);
            var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            var DeleteStatus = await _reviewssManager.DeleteByIdAsync(deleteReviewRequest.ReviewId, currentUserId, claim?.Value ?? UserRoles.Client.ToString());


            if (deleteReviewRequest.ReportId is not null && claim?.Value == UserRoles.Admin.ToString())
            {
                var insertReportActionDto = new InserReportActionDto(DeleteStatus.StatusMessage, deleteReviewRequest.ReportId.Value, currentUserId);
                var addingReportAction = await _reportActionManager.AddReportAction(insertReportActionDto);

                return StatusCode((int)addingReportAction.StatusCode, DeleteStatus);
            }

            return StatusCode((int)DeleteStatus.StatusCode, DeleteStatus);
        }
    }
}
