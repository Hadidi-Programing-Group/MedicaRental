using MedicaRental.BLL.Dtos.SubCategory;
using MedicaRental.BLL.Dtos;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers;

public class SubCategoriesManager : ISubCategoriesManager
{
    private readonly IUnitOfWork unitOfWork;

    public SubCategoriesManager(IUnitOfWork _unitOfWork)
    {
        unitOfWork = _unitOfWork;
    }
    public async Task<DeleteSubCategoryStatusDto> DeleteByIdAsync(Guid id)
    {
        await unitOfWork.SubCategories.DeleteOneById(id);
        try
        {
            unitOfWork.Save();
            return new DeleteSubCategoryStatusDto(
                isDeleted: true,
                StatusMessage: "SubCategory has been deleted successfully.");
        }

        catch
        {
            return new DeleteSubCategoryStatusDto(
                isDeleted: false,
                StatusMessage: "Unable to delete the subcategory because it has items!"
                );
        }
    }

    public async Task<IEnumerable<SubCategoriesDto>> GetAllAsync()
    {
        var SubCategories = await unitOfWork.SubCategories.GetAllAsync();
        return SubCategories.Select(SubCat => new SubCategoriesDto(
            Id: SubCat.Id,
            Name: SubCat.Name,
            Icon: SubCat.Icon,
            CategoryId: SubCat.CategoryId));
    }

    public async Task<SubCategoriesDto?> GetByIdAsync(Guid? id)
    {
        var SubCategory = await unitOfWork.SubCategories.FindAsync(
            predicate: S => S.Id == id);
        if (SubCategory is null)
            return null;
        return new SubCategoriesDto(
            Id: SubCategory.Id,
            Name: SubCategory.Name,
            Icon: SubCategory.Icon,
            CategoryId: SubCategory.CategoryId);
    }


    public async Task<InsertSubCategoryStatusDto> InsertSubCategory(InsertSubCategoryDto insertSubCategory)
    {
        var Category = await unitOfWork.Categories.FindAsync(
            predicate: C => C.Id == insertSubCategory.CategoryId);
        if (Category is null)
        {
            return new InsertSubCategoryStatusDto(
                isCreated: false,
                Id: null,
                StatusMessage: "There is no category with such id!");
        }

        SubCategory subCategory = new()
        {
            Name = insertSubCategory.Name,
            Icon = Convert.FromBase64String(insertSubCategory.Icon),
            CategoryId = insertSubCategory.CategoryId
        };

        await unitOfWork.SubCategories.AddAsync(subCategory);
        try
        {
            unitOfWork.Save();
            return new InsertSubCategoryStatusDto(
                isCreated: true,
                Id: subCategory.Id,
                StatusMessage: "Subcategory has been created successfully.");
        }

        catch
        {
            return new InsertSubCategoryStatusDto(
                isCreated: false,
                Id: null,
                StatusMessage: "Failed to insert subcategory!");
        }


    }

    public async Task<UpdateSubCategoryStatusDto> UpdateSubCategory(Guid id, UpdateSubCategoryDto updateSubCategory)
    {
        var SubCat = await unitOfWork.SubCategories.FindAsync(predicate: C => C.Id == id);
        if (SubCat is null)
        {
            return new UpdateSubCategoryStatusDto(
                isUpdated: false,
                Id: null,
                StatusMessage: "SubCategory cannot be found!");
        }

        var Category = await unitOfWork.Categories.FindAsync(
            predicate: C => C.Id == updateSubCategory.CategoryId);
        if (Category is null)
        {
            return new UpdateSubCategoryStatusDto(
                isUpdated: false,
                Id: null,
                StatusMessage: "There is no category with such id!");
        }

        SubCat.Name = updateSubCategory.Name;
        SubCat.Icon = Convert.FromBase64String(updateSubCategory.Icon);
        SubCat.CategoryId = updateSubCategory.CategoryId;

        unitOfWork.SubCategories.Update(SubCat);

        try
        {
            unitOfWork.Save();
            return new UpdateSubCategoryStatusDto(
                isUpdated: true,
                Id: SubCat.Id,
                StatusMessage: "SubCategory has been updated successfully.");

        }

        catch
        {
            return new UpdateSubCategoryStatusDto(
                isUpdated: false,
                Id: null,
                StatusMessage: "Unable to update subcategory!"
                );
        }
    }
}
