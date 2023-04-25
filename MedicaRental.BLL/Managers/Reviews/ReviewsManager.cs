using MedicaRental.BLL.Dtos;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers;

public class ReviewsManager : IReviewsManager
{
    private readonly IUnitOfWork unitOfWork;

    public ReviewsManager(IUnitOfWork _unitOfWork)
    {
        unitOfWork = _unitOfWork;
    }

    public async Task<DeleteReviewStatusDto> DeleteByIdAsync(Guid id)
    {
        await unitOfWork.Reviews.DeleteOneById(id);
        try
        {
            unitOfWork.Save();
            return new DeleteReviewStatusDto(
                isDeleted: true,
                StatusMessage: "Review has been deleted successfully.");
        }

        catch
        {
            return new DeleteReviewStatusDto(
                isDeleted: false,
                StatusMessage: "Unable to delete the review!"
                );
        }
    }

    public async Task<IEnumerable<ReviewDto>> GetAllAsync()
    {
        var reviews = await unitOfWork.Reviews.GetAllAsync(
                selector: rev => new ReviewDto
                (
                    rev.Id,
                    rev.Rating,
                    rev.IsDeleted,
                    rev.ClientReview,
                    rev.ClientId,
                    rev.SellerId,
                    rev.ItemId
                )
            );

        return reviews;
    }

    public async Task<ReviewDto?> GetByIdAsync(Guid? id)
    {
        var review = await unitOfWork.Reviews.FindAsync(
            predicate: S => S.Id == id);
        if (review is null)
            return null;
        return new ReviewDto
                (
                    review.Id,
                    review.Rating,
                    review.IsDeleted,
                    review.ClientReview,
                    review.ClientId,
                    review.SellerId,
                    review.ItemId
                );
    }

    public async Task<InsertReviewStatusDto> InsertReview(InsertReviewDto insertReview)
    {
        var client = await unitOfWork.Clients.FindAsync(
            predicate: C => C.Id == insertReview.ClientId);
        if (client is null)
        {
            return new InsertReviewStatusDto(
                isCreated: false,
                Id: null,
                StatusMessage: "There is no client with such id!");
        }

        var seller = await unitOfWork.Clients.FindAsync(
            predicate: C => C.Id == insertReview.SellerId);
        if (seller is null)
        {
            return new InsertReviewStatusDto(
                isCreated: false,
                Id: null,
                StatusMessage: "There is no seller with such id!");
        }

        var Item = await unitOfWork.Items.FindAsync(
            predicate: C => C.Id == insertReview.ItemId);
        if (Item is null)
        {
            return new InsertReviewStatusDto(
                isCreated: false,
                Id: null,
                StatusMessage: "There is no Item with such id!");
        }

        var RentOps = await unitOfWork.RentOperations.FindAllAsync(
            predicate: R => R.ClientId==insertReview.ClientId&&R.ItemId==insertReview.ItemId&&R.ReviewId==null);
        var RentOp = RentOps.LastOrDefault();
        if (RentOp is null || RentOps is null)
        {
            return new InsertReviewStatusDto(
                isCreated: false,
                Id: null,
                StatusMessage: "There is no rent operation with such id!");
        }


        Review review = new()
        {
            Rating = insertReview.Rating,
            IsDeleted = insertReview.IsDeleted,
            ClientReview = insertReview?.ClientReview ?? string.Empty,
            ClientId = insertReview!.ClientId!,
            SellerId = insertReview!.SellerId!,
            ItemId = insertReview.ItemId,
            RentOperationId = RentOp.Id          
        };

        await unitOfWork.Reviews.AddAsync(review);
        try
        {
            unitOfWork.Save();
            RentOp.ReviewId = review.Id;
            unitOfWork.RentOperations.Update(RentOp);
            unitOfWork.Save();
            return new InsertReviewStatusDto(
                isCreated: true,
                Id: review.Id,
                StatusMessage: "Review has been created successfully.");
        }

        catch
        {
            return new InsertReviewStatusDto(
                isCreated: false,
                Id: null,
                StatusMessage: "Failed to insert Review!");
        }
    }
}
