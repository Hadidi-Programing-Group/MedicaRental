﻿using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedicaRental.BLL.Dtos;

namespace MedicaRental.BLL.Managers;

public interface IItemsManager
{
    public Task<PageDto<HomeItemDto>?> GetAllItemsAsync(int page, string? orderBy);
    public Task<PageDto<HomeItemDto>?> GetItemsBySellerAsync(string sellerId, int page, string? orderBy);
    public Task<PageDto<HomeItemDto>?> GetItemsBySearchAsync(string searchText, int page, string? orderBy);
    public Task<PageDto<HomeItemDto>?> GetItemsByCategoryAsync(Guid categoryId, int page, string? orderBy);
    public Task<PageDto<HomeItemDto>?> GetItemsBySubCategoryAsync(Guid subcategoryId, int page, string? orderBy);
    public Task<PageDto<HomeItemDto>?> GetItemsByCategoriesAsync(IEnumerable<Guid> categoryIds, int page, string? orderBy);
    public Task<PageDto<HomeItemDto>?> GetItemsBySubCategoriesAsync(IEnumerable<Guid> subcategoryIds, int page, string? orderBy);

    public Task<PageDto<SellerItemDto>?> GetAllItemsForSellerAsync(int page, string? orderBy);

    public Task<PageDto<RenterItemDto>?> GetAllItemsForRenterAsync(int page, string? orderBy);

    public Task<HomeItemDto?> FindItemAsync(Guid id);
    public Task<SellerItemDto?> FindItemForSellerAsync(Guid id);
    public Task<RenterItemDto?> FindItemForRenterAsync(Guid id);

    public Task<StatusDto> AddItemAsync(AddItemDto item);
    public Task<StatusDto> AddItemsAsync(IEnumerable<AddItemDto> items);
    
    public Task<StatusDto> UpdateItem(UpdateItemDto item);
    public Task<StatusDto> UpdateItems(IEnumerable<UpdateItemDto> items);
    
    public Task<StatusDto> DeleteItem(Guid id);
    public Task<StatusDto> DeleteItems(IEnumerable<Guid> ids);

    public Task<StatusDto> UnListItem(Guid id);
}
