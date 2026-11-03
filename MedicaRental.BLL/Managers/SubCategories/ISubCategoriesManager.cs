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
    Task<IEnumerable<SubCategoriesDto>> GetAllAsync();
    Task<SubCategoriesDto?> GetByIdAsync(Guid? id);
    Task<DeleteSubCategoryStatusDto> DeleteByIdAsync(Guid id);
    Task<UpdateSubCategoryStatusDto> UpdateSubCategory(Guid id, UpdateSubCategoryDto updateSubCategory);
    Task<InsertSubCategoryStatusDto> InsertSubCategory(InsertSubCategoryDto insertSubCategory);

}
