using MedicaRental.BLL.Dtos.SubCategory;
using MedicaRental.BLL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers;

public interface IReviewsManager
{
    Task<IEnumerable<ReviewDto>> GetAllAsync();
    Task<ReviewDto?> GetByIdAsync(Guid? id);
    Task<DeleteReviewStatusDto> DeleteByIdAsync(Guid id);
    Task<InsertReviewStatusDto> InsertReview(InsertReviewDto insertReview);
}
