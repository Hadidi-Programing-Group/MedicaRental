using MedicaRental.DAL.UnitOfWork;
using MedicaRental.BLL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MedicaRental.DAL.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Collections;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using System.Xml.Linq;
using System.Net;

namespace MedicaRental.BLL.Managers;

public class ItemsManager : IItemsManager
{
    private readonly IUnitOfWork _unitOfWork;

    public ItemsManager(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<StatusDto> AddItemAsync(AddItemDto item)
    {
        Item _item = new()
        {
            Name = item.Name,
            Description = item.Description,
            Serial = item.Serial,
            Model = item.Model,
            Make = item.Make,
            Country = item.Country,
            Stock = item.Stock,
            Price = item.Price,
            Image = Convert.FromBase64String(item.Image),
            CategoryId = item.CategoryId,
            SubCategoryId = item.SubCategoryId,
            SellerId = item.SellerId
        };

        try
        {
            await _unitOfWork.Items.AddAsync(_item);
            _unitOfWork.Save();
            return new InsertStatusDto("Item added successfully", HttpStatusCode.Created, _item.Id);
        }
        catch (Exception ex)
        {
            return new StatusDto (ex.Message, HttpStatusCode.BadRequest);
        }
    }

    public async Task<StatusDto> AddItemsAsync(IEnumerable<AddItemDto> items)
    {
        List<Item> _items = new();
        foreach (var item in items)
        {
            Item _item = new()
            {
                Name = item.Name,
                Description = item.Description,
                Serial = item.Serial,
                Model = item.Model,
                Make = item.Make,
                Country = item.Country,
                Stock = item.Stock,
                Price = item.Price,
                Image = Convert.FromBase64String(item.Image),
                CategoryId = item.CategoryId,
                SubCategoryId = item.SubCategoryId,
                SellerId = item.SellerId
            };

            _items.Add(_item);
        }

        await _unitOfWork.Items.AddRangeAsync(_items);

        try
        {
            await _unitOfWork.Items.AddRangeAsync(_items);
            _unitOfWork.Save();
            return new StatusDto("Item added successfully", HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return new StatusDto(ex.Message, HttpStatusCode.BadRequest);
        }
    }

    public async Task<StatusDto> DeleteItem(int id)
    {
        var succeeded = await _unitOfWork.Items.DeleteOneById(id);

        if(!succeeded) return new StatusDto("No item with the given id.", HttpStatusCode.NotFound);

        try
        {
            _unitOfWork.Save();
            return new StatusDto("Item deleted successfully", HttpStatusCode.NoContent);
        }
        catch (Exception ex)
        {
            return new StatusDto($"Item couldn't be deleted.\nCause: {ex.Message}", HttpStatusCode.BadRequest);
        }
    }

    public async Task<StatusDto> DeleteItems(IEnumerable<int> ids)
    {
        var failed = await _unitOfWork.Items.DeleteManyById(ids);

        if (failed.Count > 0) return new StatusDto($"{failed.Count} item ids couldn't be found.\noperation aborted.", HttpStatusCode.NotFound);

        try
        {
            _unitOfWork.Save();
            return new StatusDto("Items deleted successfully", HttpStatusCode.NoContent);
        }
        catch (Exception ex)
        {
            return new StatusDto($"Items couldn't be deleted.\nCause: {ex.Message}", HttpStatusCode.BadRequest);
        }
    }

    public Task<StatusDto> UpdateItem(UpdateItemDto item)
    {
        throw new NotImplementedException();
    }

    public Task<StatusDto> UpdateItems()
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
}
