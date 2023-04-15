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
    public Task<IEnumerable<HomeItemDto>> GetAllItemsAsync();
    public Task<IEnumerable<HomeItemDto>> GetItemsBySellerAsync(string sellerId);
    public Task<IEnumerable<HomeItemDto>> GetItemsBySearchAsync(string searchText);
    public Task<IEnumerable<HomeItemDto>> GetItemsByCategoryAsync(int categoryId);
    public Task<IEnumerable<HomeItemDto>> GetItemsBySubCategoryAsync(int subcategoryId);
    public Task<IEnumerable<HomeItemDto>> GetItemsByCategoriesAsync(IEnumerable<int> categoryIds);
    public Task<IEnumerable<HomeItemDto>> GetItemsBySubCategoriesAsync(IEnumerable<int> subcategoryIds);

    public Task<IEnumerable<SellerItemDto>> GetAllItemsForSellerAsync();

    public Task<IEnumerable<RenterItemDto>> GetAllItemsForRenterAsync();

    public Task<HomeItemDto> FindItemAsync();
    public Task<SellerItemDto> FindItemForSellerAsync();
    public Task<RenterItemDto> FindItemForRenterAsync();

    public Task<StatusDto> AddItemAsync(AddItemDto item);
    public Task<StatusDto> AddItemsAsync(IEnumerable<AddItemDto> items);
    
    public Task<StatusDto> UpdateItem(UpdateItemDto item);
    public Task<StatusDto> UpdateItems();
    
    public Task<StatusDto> DeleteItem(int id);
    public Task<StatusDto> DeleteItems(IEnumerable<int> ids);
}
