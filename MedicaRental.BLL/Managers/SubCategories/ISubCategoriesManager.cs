using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.SubCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers;

public interface ISubCategoriesManager
{
    Task<IEnumerable<SubCategoriesDto>> GetAllAsync();
    Task<SubCategoriesDto?> GetByIdAsync(int? id);
    Task<DeleteSubCategoryStatusDto> DeleteByIdAsync(int id);
    Task<UpdateSubCategoryStatusDto> UpdateSubCategory(int id, UpdateSubCategoryDto updateSubCategory);
    Task<InsertSubCategoryStatusDto> InsertSubCategory(InsertSubCategoryDto insertSubCategory);
}
