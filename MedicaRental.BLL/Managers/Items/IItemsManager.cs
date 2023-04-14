using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedicaRental.BLL.Dtos.Item;

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

    public void AddItemAsync(AddItemDto item);
    public void AddItemsAsync(IEnumerable<AddItemDto> items);

    public void UpdateItem(UpdateItemDto item);
    public void UpdateItems();

    public void DeleteOneItemById(int id);
    public void DeleteManyItemsById(IEnumerable<int> ids);
    public void DeleteItem(DeleteItemDto item);
    public void DeleteItems(IEnumerable<DeleteItemDto> items);
}
