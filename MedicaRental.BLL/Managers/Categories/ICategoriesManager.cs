using MedicaRental.BLL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers;

public interface ICategoriesManager
{
    Task<DeleteCategoryStatusDto> DeleteByIdAsync(Guid id);
    Task<IEnumerable<CategoryDto>> GetAllAsyc();
    Task<CategoryWithSubCategoriesDto?> GetCategoryWithSubCategories(Guid id);
    Task<InsertCategoryStatusDto> InsertNewCategory(InsertCategoryDto insertCategoryDto);
    Task<UpdateCategoryStatusDto> UpdateNewCategory(Guid id, UpdateCategoryDto updateCategoryDto);
}
