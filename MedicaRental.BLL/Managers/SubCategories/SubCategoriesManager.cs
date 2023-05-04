using MedicaRental.BLL.Dtos.SubCategory;
using MedicaRental.BLL.Dtos;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedicaRental.DAL.Context;
using MedicaRental.BLL.Helpers;
using Microsoft.EntityFrameworkCore;

namespace MedicaRental.BLL.Managers;

public class SubCategoriesManager : ISubCategoriesManager
{
    private readonly IUnitOfWork _unitOfWork;

    public SubCategoriesManager(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteSubCategoryStatusDto> DeleteByIdAsync(Guid id)
    {
        await _unitOfWork.SubCategories.DeleteOneById(id);
        try
        {
            _unitOfWork.Save();
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

    public async Task<PageDto<SubCategoryWithCategoryDto>> GetAllWithCategoryAsync(int page, string? searchText)
    {
        var subCategories = await _unitOfWork.SubCategories.FindAllAsync(
                predicate: c => searchText == null ||
                MedicaRentalDbContext.LevDist(c.Name, searchText, SharedHelper.SearchMaxDistance) <= SharedHelper.SearchMaxDistance,
                include: q => q.Include(sc => sc.Category),
                orderBy: q => q.OrderBy(c => c.Name),
                skip: page > 1 ? (page - 1) * SharedHelper.Take : null,
                take: SharedHelper.Take,
                selector: sc => new SubCategoryWithCategoryDto
                (
                    sc.Id,
                    sc.Name,
                    SharedHelper.GetMimeFromBase64(Convert.ToBase64String(sc.Icon ?? Array.Empty<byte>())),
                    sc.CategoryId,
                    sc.Category == null ? "" : sc.Category.Name
                )
            );

        var count = await _unitOfWork.SubCategories.GetCountAsync
               (
                   c => searchText == null ||
                   MedicaRentalDbContext.LevDist(c.Name, searchText, SharedHelper.SearchMaxDistance) <= SharedHelper.SearchMaxDistance
               );

        return new(subCategories, count);
    }

    public async Task<SubCategoryWithCategoryDto?> GetByIdAsync(Guid? id)
    {
        var subCategory = await _unitOfWork.SubCategories.FindAsync
            (
                predicate: S => S.Id == id,
                include: q => q.Include(sc => sc.Category)
            );

        if (subCategory is null)
            return null;
        
        return new SubCategoryWithCategoryDto(
            Id: subCategory.Id,
            Name: subCategory.Name,
            Icon: SharedHelper.GetMimeFromBase64(Convert.ToBase64String(subCategory.Icon ?? Array.Empty<byte>())),
            CategoryId: subCategory.CategoryId,
            CategoryName: subCategory.Category == null ? "" : subCategory.Category.Name);
    }


    public async Task<InsertSubCategoryStatusDto> InsertSubCategory(InsertSubCategoryDto insertSubCategory)
    {
        var Category = await _unitOfWork.Categories.FindAsync(
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

        await _unitOfWork.SubCategories.AddAsync(subCategory);
        try
        {
            _unitOfWork.Save();
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

    public async Task<UpdateSubCategoryStatusDto> UpdateSubCategory(UpdateSubCategoryDto updateSubCategory)
    {
        var SubCat = await _unitOfWork.SubCategories.FindAsync(predicate: C => C.Id == updateSubCategory.Id);
        if (SubCat is null)
        {
            return new UpdateSubCategoryStatusDto(
                isUpdated: false,
                Id: null,
                StatusMessage: "SubCategory cannot be found!");
        }

        var Category = await _unitOfWork.Categories.FindAsync(
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

        _unitOfWork.SubCategories.Update(SubCat);

        try
        {
            _unitOfWork.Save();
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
