using MedicaRental.DAL.UnitOfWork;
using MedicaRental.BLL.Dtos.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers;

public class ItemsManager : IItemsManager
{
    private readonly IUnitOfWork _unitOfWork;

    public ItemsManager(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public void AddItemAsync(AddItemDto item)
    {
        throw new NotImplementedException();
    }

    public void AddItemsAsync(IEnumerable<AddItemDto> items)
    {
        throw new NotImplementedException();
    }

    public void DeleteItem(DeleteItemDto item)
    {
        throw new NotImplementedException();
    }

    public void DeleteItems(IEnumerable<DeleteItemDto> items)
    {
        throw new NotImplementedException();
    }

    public void DeleteManyItemsById(IEnumerable<int> ids)
    {
        throw new NotImplementedException();
    }

    public void DeleteOneItemById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<HomeItemDto> FindItemAsync()
    {
        throw new NotImplementedException();
    }

    public Task<RenterItemDto> FindItemForRenterAsync()
    {
        throw new NotImplementedException();
    }

    public Task<SellerItemDto> FindItemForSellerAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<HomeItemDto>> GetAllItemsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<RenterItemDto>> GetAllItemsForRenterAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<SellerItemDto>> GetAllItemsForSellerAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<HomeItemDto>> GetItemsByCategoriesAsync(IEnumerable<int> categoryIds)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<HomeItemDto>> GetItemsByCategoryAsync(int categoryId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<HomeItemDto>> GetItemsBySearchAsync(string searchText)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<HomeItemDto>> GetItemsBySellerAsync(string sellerId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<HomeItemDto>> GetItemsBySubCategoriesAsync(IEnumerable<int> subcategoryIds)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<HomeItemDto>> GetItemsBySubCategoryAsync(int subcategoryId)
    {
        throw new NotImplementedException();
    }

    public void UpdateItem(UpdateItemDto item)
    {
        throw new NotImplementedException();
    }

    public void UpdateItems()
    {
        throw new NotImplementedException();
    }
}
