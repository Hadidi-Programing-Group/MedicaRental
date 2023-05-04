using MedicaRental.BLL.Dtos.SubCategory;
using MedicaRental.BLL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers;

public interface ISubCategoriesManager
{
    Task<PageDto<SubCategoryWithCategoryDto>> GetAllWithCategoryAsync(int page, string? searchText);
    Task<SubCategoryWithCategoryDto?> GetByIdAsync(Guid? id);
    Task<DeleteSubCategoryStatusDto> DeleteByIdAsync(Guid id);
    Task<UpdateSubCategoryStatusDto> UpdateSubCategory(UpdateSubCategoryDto updateSubCategory);
    Task<InsertSubCategoryStatusDto> InsertSubCategory(InsertSubCategoryDto insertSubCategory);

}
