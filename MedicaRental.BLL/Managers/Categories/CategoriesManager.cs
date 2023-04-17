using MedicaRental.BLL.Dtos;
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

    public async Task<IEnumerable<CategoryWithSubCategoriesDto>> GetAllAsyc()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync(
            include: source => source.Include(category => category.SubCategories),
            selector: category => new CategoryWithSubCategoriesDto(
                category.Id,
                category.Name,
                category.Icon,
                category.SubCategories.Select(sc => new SubCategoryDto(
                sc.Id,
                sc.Name,
                sc.Icon
                )).ToList()
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
            Icon: category.Icon,
            SubCategories: category.SubCategories.Select(sc => new SubCategoryDto(
                Id: sc.Id,
                Name: sc.Name,
                Icon: sc.Icon
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
            return new InsertCategoryStatusDto(
                isCreated: true,
                Id: category.Id,
                StatusMessage: "Category Created Successfully"
                );
        }
        catch
        {
            return new InsertCategoryStatusDto(
                isCreated: false,
                Id: null,
                StatusMessage: "Failed to insert new Category"
                );
        }
    }

    public async Task<UpdateCategoryStatusDto> UpdateNewCategory(Guid id, UpdateCategoryDto updateCategoryDto)
    {
        var category = await _unitOfWork.Categories.FindAsync(c => c.Id == id);
        if (category is null)
            return new UpdateCategoryStatusDto(
                isUpdated: false,
                Id: null,
                StatusMessage: "Category Couldn't be found"
                );

        category.Name = updateCategoryDto.Name;
        category.Icon = Convert.FromBase64String(updateCategoryDto.Icon);

        _unitOfWork.Categories.Update(category);

        try
        {
            _unitOfWork.Save();
            return new UpdateCategoryStatusDto(
                isUpdated: true,
                Id: category.Id,
                StatusMessage: "Category was updated successfully"
                );
        }
        catch
        {
            return new UpdateCategoryStatusDto(
                isUpdated: false,
                Id: null,
                StatusMessage: "Category Couldn't be updated"
                );
        }
    }
}
