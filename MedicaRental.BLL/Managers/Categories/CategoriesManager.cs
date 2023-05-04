using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Helpers;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers;

public class CategoriesManager : ICategoriesManager
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoriesManager(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteCategoryStatusDto> DeleteByIdAsync(Guid id)
    {
        await _unitOfWork.Categories.DeleteOneById(id);
        try
        {
            _unitOfWork.Save();
            return new DeleteCategoryStatusDto(
                isDeleted: true,
                StatusMessage: "Category was deleted successfully"
                );
        }
        catch
        {
            return new DeleteCategoryStatusDto(
                isDeleted: false,
                StatusMessage: "Coudn't remove category, There are items in this category"
                );
        }
    }

    public async Task<IEnumerable<CategoryWithSubCategoriesDto>> GetAllWithSubCategoriesAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync(
            include: source => source.Include(category => category.SubCategories),
            selector: category => new CategoryWithSubCategoriesDto(
                category.Id,
                category.Name,
                SharedHelper.GetMimeFromBase64(Convert.ToBase64String(category.Icon ?? Array.Empty<byte>())),
                category.SubCategories.Select(sc => new SubCategoryWithCategoryDto(
                sc.Id,
                sc.Name,
                SharedHelper.GetMimeFromBase64(Convert.ToBase64String(sc.Icon ?? Array.Empty<byte>())),
                sc.CategoryId,
                category.Name
                )).ToList()
            )
        );

        return categories;
    }

    public async Task<PageDto<CategoryDto>> GetAllAsync(int page, string? searchText)
    {
        var categories = await _unitOfWork.Categories.FindAllAsync(
            predicate: c => searchText == null ||
            MedicaRentalDbContext.LevDist(c.Name, searchText, SharedHelper.SearchMaxDistance) <= SharedHelper.SearchMaxDistance,
            orderBy: searchText == null ? q => q.OrderBy(c => c.Name) :
            q => q.OrderBy(c => MedicaRentalDbContext.LevDist(c.Name, searchText, SharedHelper.SearchMaxDistance)).ThenBy(c => c.Name),
            skip: page > 1 ? (page - 1) * SharedHelper.Take : null,
            take: SharedHelper.Take,
            selector: category => new CategoryDto(
                category.Id,
                category.Name,
                SharedHelper.GetMimeFromBase64(Convert.ToBase64String(category.Icon ?? Array.Empty<byte>()))
            )
        );

        var count = await _unitOfWork.Categories.GetCountAsync
                (
                    c => searchText == null ||
                    MedicaRentalDbContext.LevDist(c.Name, searchText, SharedHelper.SearchMaxDistance) <= SharedHelper.SearchMaxDistance
                );

        return new PageDto<CategoryDto>(categories, count);
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await _unitOfWork.Categories.FindAllAsync(
            orderBy: q => q.OrderBy(c => c.Name),
            selector: category => new CategoryDto(
                category.Id,
                category.Name,
                SharedHelper.GetMimeFromBase64(Convert.ToBase64String(category.Icon ?? Array.Empty<byte>()))
            )
        );


        return categories;
    }

    public async Task<CategoryWithSubCategoriesDto?> GetCategoryWithSubCategories(Guid id)
    {
        var category = await _unitOfWork.Categories.FindAsync(
            predicate: (c) => c.Id == id,
            include: source => source.Include(c => c.SubCategories)
            );

        if (category is null)
            return null;

        return new CategoryWithSubCategoriesDto(
            Id: category.Id,
            Name: category.Name,
            Icon: SharedHelper.GetMimeFromBase64(Convert.ToBase64String(category.Icon ?? Array.Empty<byte>())),
            SubCategories: category.SubCategories.Select(sc => new SubCategoryWithCategoryDto(
                Id: sc.Id,
                Name: sc.Name,
                Icon: SharedHelper.GetMimeFromBase64(Convert.ToBase64String(sc.Icon ?? Array.Empty<byte>())),
                CategoryId: sc.CategoryId,
                CategoryName: category.Name
                )).ToList()
            );
    }

    public async Task<InsertCategoryStatusDto> InsertNewCategory(InsertCategoryDto insertCategoryDto)
    {

        Category category = new()
        {
            Name = insertCategoryDto.Name,
            Icon = Convert.FromBase64String(insertCategoryDto.Icon)
        };


        await _unitOfWork.Categories.AddAsync(category);
        try
        {
            _unitOfWork.Save();
        }
        catch
        {
            return new InsertCategoryStatusDto(
                isCreated: false,
                Id: null,
                StatusMessage: "Failed to insert new Category"
                );
        }

        try
        {
            _unitOfWork.Save();
            return new InsertCategoryStatusDto(
               isCreated: true,
               Id: category.Id,
               StatusMessage: "Category Created Successfully"
               );
        }
        catch
        {
            return new InsertCategoryStatusDto(
                isCreated: true,
                Id: null,
                StatusMessage: "Failed to insert new Category"
                );
        }
    }

    public async Task<UpdateCategoryStatusDto> UpdateNewCategory(UpdateCategoryDto updateCategoryDto)
    {
        var category = await _unitOfWork.Categories.FindAsync
            (
                c => c.Id == updateCategoryDto.Id,
                disableTracking: false
            );

        if (category is null)
        {
            return new UpdateCategoryStatusDto
                (
                    isUpdated: false,
                    StatusMessage: "Category Couldn't be found"
                );
        }

        category.Name = updateCategoryDto.Name;
        category.Icon = Convert.FromBase64String(updateCategoryDto.Icon);

        var succeeded = _unitOfWork.Categories.Update(category);

        if (!succeeded)
        {
            return new UpdateCategoryStatusDto(
                isUpdated: false,
                StatusMessage: "Category Couldn't be updated"
                );
        }

        try
        {
            _unitOfWork.Save();
            return new UpdateCategoryStatusDto
               (
                   isUpdated: true,
                   StatusMessage: "Category updated Successfully"
               );
        }
        catch
        {
            return new UpdateCategoryStatusDto
                (
                   isUpdated: false,
                   StatusMessage: "Category Couldn't be updated"
                );
        }
    }
}
