using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedicaRental.BLL.Dtos;

namespace MedicaRental.BLL.Managers;

public interface IItemsManager
{
    public Task<IEnumerable<HomeItemDto>?> GetAllItemsAsync(string? orderBy);
    public Task<IEnumerable<HomeItemDto>?> GetItemsBySellerAsync(string sellerId, string? orderBy);
    public Task<IEnumerable<HomeItemDto>?> GetItemsBySearchAsync(string searchText, string? orderBy);
    public Task<IEnumerable<HomeItemDto>?> GetItemsByCategoryAsync(Guid categoryId, string? orderBy);
    public Task<IEnumerable<HomeItemDto>?> GetItemsBySubCategoryAsync(Guid subcategoryId, string? orderBy);
    public Task<IEnumerable<HomeItemDto>?> GetItemsByCategoriesAsync(IEnumerable<Guid> categoryIds, string? orderBy);
    public Task<IEnumerable<HomeItemDto>?> GetItemsBySubCategoriesAsync(IEnumerable<Guid> subcategoryIds, string? orderBy);

    public Task<IEnumerable<SellerItemDto>?> GetAllItemsForSellerAsync(string? orderBy);

    public Task<IEnumerable<RenterItemDto>?> GetAllItemsForRenterAsync(string? orderBy);

    public Task<HomeItemDto?> FindItemAsync(Guid id);
    public Task<SellerItemDto?> FindItemForSellerAsync(Guid id);
    public Task<RenterItemDto?> FindItemForRenterAsync(Guid id);

    public Task<StatusDto> AddItemAsync(AddItemDto item);
    public Task<StatusDto> AddItemsAsync(IEnumerable<AddItemDto> items);
    
    public Task<StatusDto> UpdateItem(UpdateItemDto item);
    public Task<StatusDto> UpdateItems(IEnumerable<UpdateItemDto> items);
    
    public Task<StatusDto> DeleteItem(Guid id);
    public Task<StatusDto> DeleteItems(IEnumerable<Guid> ids);
}
