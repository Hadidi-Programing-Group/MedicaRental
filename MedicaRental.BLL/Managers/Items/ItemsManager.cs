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
using MedicaRental.BLL.Helpers;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using MedicaRental.DAL.Repositories;

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
        Item _item = ItemHelper.MapAddDto(item);

        var success = await _unitOfWork.Items.AddAsync(_item);

        if (!success)
            return new StatusDto("Item couldn't be added", HttpStatusCode.BadRequest);

        try
        {
            _unitOfWork.Save();
            return new InsertStatusDto("Item added successfully", HttpStatusCode.Created, _item.Id);
        }
        catch (Exception ex)
        {
            return new StatusDto($"Item couldn't be added.\nCause: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<StatusDto> AddItemsAsync(IEnumerable<AddItemDto> items)
    {
        List<Item> _items = new();
        foreach (var item in items)
        {
            Item _item = ItemHelper.MapAddDto(item);

            _items.Add(_item);
        }

        var success = await _unitOfWork.Items.AddRangeAsync(_items);

        if (!success)
            return new StatusDto("Items couldn't be added", HttpStatusCode.BadRequest);

        try
        {
            _unitOfWork.Save();
            return new StatusDto("Items added successfully", HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return new StatusDto($"Items couldn't be added.\nCause: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<StatusDto> DeleteItem(Guid id)
    {
        var item = await _unitOfWork.Items.FindAsync(predicate: i => i.Id == id, include: source => source.Include(i => i.ItemRenters), disableTracking: false);

        if (item is null)
            return new StatusDto("No item with the given id.", HttpStatusCode.NotFound);

        if (item.IsListed)
            return new StatusDto("Can't delete a listed item.", HttpStatusCode.BadRequest);

        var hasRenters = ((IItemsRepo)_unitOfWork).HasRenters(item);

        if (hasRenters)
            return new StatusDto("Can't delete an item that has active renters.", HttpStatusCode.BadRequest);

        _unitOfWork.Items.Delete(item);

        try
        {
            _unitOfWork.Save();
            return new StatusDto("Item deleted successfully", HttpStatusCode.NoContent);
        }
        catch (Exception ex)
        {
            return new StatusDto($"Item couldn't be deleted.\nCause: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<StatusDto> DeleteItems(IEnumerable<Guid> ids)
    {
        var items = await _unitOfWork.Items.FindAllAsync(predicate: i => ids.Contains(i.Id), include: source => source.Include(i => i.ItemRenters), disableTracking: false);

        if (items.Count() != ids.Count())
            return new StatusDto("Some or all items coudn't be found.", HttpStatusCode.NotFound);

        if (items.Any(i => i.IsListed))
            return new StatusDto("Can't delete a listed item.", HttpStatusCode.BadRequest);

        var hasRenters = ((IItemsRepo)_unitOfWork).HasRenters(items);

        if (hasRenters)
            return new StatusDto("Can't delete an item that has active renters.", HttpStatusCode.BadRequest);

        _unitOfWork.Items.DeleteRange(items);

        try
        {
            _unitOfWork.Save();
            return new StatusDto("Items deleted successfully", HttpStatusCode.NoContent);
        }
        catch (Exception ex)
        {
            return new StatusDto($"Items couldn't be deleted.\nCause: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<StatusDto> UpdateItem(UpdateItemDto item)
    {
        var _item = await _unitOfWork.Items.FindAsync(predicate: i => i.Id == item.Id, disableTracking: false);

        if (_item is null) return new("Item doesn't exist.", HttpStatusCode.NotFound);

        _item = ItemHelper.MapUpdateDto(_item, item);

        var success = _unitOfWork.Items.Update(_item);

        if (!success)
            return new StatusDto("Item couldn't be updated", HttpStatusCode.BadRequest);

        try
        {
            _unitOfWork.Save();
            return new StatusDto("Item updated successfully", HttpStatusCode.NoContent);
        }
        catch (Exception ex)
        {
            return new StatusDto($"Item couldn't be updated.\nCause: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<StatusDto> UpdateItems(IEnumerable<UpdateItemDto> items)
    {
        var ids = items.Select(x => x.Id).ToList();
        var _items = await _unitOfWork.Items.FindAllAsync(predicate: i => ids.Contains(i.Id), disableTracking: false);

        if (items.Count() != _items.Count())
            return new StatusDto("Some or all items coudn't be found.", HttpStatusCode.NotFound);

        List<Item> updatedItems = new();

        for (int i = 0; i < items.Count(); i++)
        {
            updatedItems.Add(ItemHelper.MapUpdateDto(_items.ElementAt(i), items.ElementAt(i)));
        }

        var success = _unitOfWork.Items.UpdateRange(updatedItems);

        if (!success)
            return new StatusDto("Items couldn't be updated", HttpStatusCode.BadRequest);

        try
        {
            _unitOfWork.Save();
            return new StatusDto("Items updated successfully", HttpStatusCode.NoContent);
        }
        catch (Exception ex)
        {
            return new StatusDto($"Item couldn't be updated.\nCause: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<HomeItemDto?> FindItemAsync(Guid id)
    {
        try
        {
            return await _unitOfWork.Items.FindAsync
                (
                    selector: ItemHelper.HomeDtoSelector,
                    predicate: i => id == i.Id,
                    include: ItemHelper.HomeDtoInclude
                );
        }
        catch (Exception) { return null; }
    }

    public async Task<RenterItemDto?> FindItemForRenterAsync(Guid id)
    {
        try
        {
            return await _unitOfWork.Items.FindAsync
                (
                    selector: ItemHelper.RenterDtoSelector,
                    predicate: i => id == i.Id,
                    include: ItemHelper.RenterDtoInclude
                );
        }
        catch (Exception) { return null; }
    }

    public async Task<SellerItemDto?> FindItemForSellerAsync(Guid id)
    {
        try
        {
            return await _unitOfWork.Items.FindAsync
                (
                    selector: ItemHelper.SellerDtoSelector,
                    predicate: i => id == i.Id,
                    include: ItemHelper.SellerDtoInclude
                );
        }
        catch (Exception) { return null; }
    }

    public async Task<PageDto<HomeItemDto>?> GetAllItemsAsync(int page, string? orderBy)
    {
        try
        {
            var orderByQuery = ItemHelper.GetOrderByQuery(orderBy);
            var data = await _unitOfWork.Items.GetAllAsync
                (
                    orderBy: orderByQuery,
                    selector: ItemHelper.HomeDtoSelector,
                    include: ItemHelper.HomeDtoInclude,
                    skip: page > 1 ? page * SharedHelper.Take : null,
                    take: SharedHelper.Take
                );

            var count = await _unitOfWork.Items.GetCountAsync();

            return new(data, count);
        }
        catch (Exception) { return null; }
    }

    public async Task<PageDto<RenterItemDto>?> GetAllItemsForRenterAsync(int page, string? orderBy)
    {
        try
        {
            var orderByQuery = ItemHelper.GetOrderByQuery(orderBy);
            var data = await _unitOfWork.Items.GetAllAsync
                (
                    orderBy: orderByQuery,
                    selector: ItemHelper.RenterDtoSelector,
                    include: ItemHelper.RenterDtoInclude,
                    skip: page > 1 ? page * SharedHelper.Take : null,
                    take: SharedHelper.Take
                );

            var count = await _unitOfWork.Items.GetCountAsync();

            return new(data, count);
        }
        catch (Exception) { return null; }
    }

    public async Task<PageDto<SellerItemDto>?> GetAllItemsForSellerAsync(int page, string? orderBy)
    {
        try
        {
            var orderByQuery = ItemHelper.GetOrderByQuery(orderBy);
            var data = await _unitOfWork.Items.GetAllAsync
                (
                    orderBy: orderByQuery,
                    selector: ItemHelper.SellerDtoSelector,
                    include: ItemHelper.SellerDtoInclude,
                    skip: page > 1 ? page * SharedHelper.Take : null,
                    take: SharedHelper.Take
                );

            var count = await _unitOfWork.Items.GetCountAsync();

            return new(data, count);
        }
        catch (Exception) { return null; }
    }

    public async Task<PageDto<HomeItemDto>?> GetItemsByCategoriesAsync(IEnumerable<Guid> categoryIds, int page, string? orderBy)
    {
        try
        {
            var orderByQuery = ItemHelper.GetOrderByQuery(orderBy);
            var data = await _unitOfWork.Items.FindAllAsync
                (
                    orderBy: orderByQuery,
                    selector: ItemHelper.HomeDtoSelector,
                    predicate: i => categoryIds.Contains(i.CategoryId),
                    include: ItemHelper.HomeDtoInclude,
                    skip: page > 1 ? page * SharedHelper.Take : null,
                    take: SharedHelper.Take
                );

            var count = await _unitOfWork.Items.GetCountAsync
                (
                    predicate: i => categoryIds.Contains(i.CategoryId)
                );

            return new(data, count);
        }
        catch (Exception) { return null; }
    }

    public async Task<PageDto<HomeItemDto>?> GetItemsByCategoryAsync(Guid categoryId, int page, string? orderBy)
    {
        try
        {
            var orderByQuery = ItemHelper.GetOrderByQuery(orderBy);
            var data = await _unitOfWork.Items.FindAllAsync
                (
                    orderBy: orderByQuery,
                    selector: ItemHelper.HomeDtoSelector,
                    predicate: i => categoryId == i.CategoryId,
                    include: ItemHelper.HomeDtoInclude,
                    skip: page > 1 ? page * SharedHelper.Take : null,
                    take: SharedHelper.Take
                );

            var count = await _unitOfWork.Items.GetCountAsync
                (
                    predicate: i => categoryId == i.CategoryId
                );

            return new(data, count);
        }
        catch (Exception) { return null; }
    }

    public async Task<PageDto<HomeItemDto>?> GetItemsBySearchAsync(string searchText, int page, string? orderBy)
    {
        try
        {
            var orderByQuery = ItemHelper.GetOrderByQueryForSearch(orderBy, searchText);
            var data = await _unitOfWork.Items.FindAllAsync
                (
                    orderBy: orderByQuery,
                    selector: ItemHelper.HomeDtoSelector,
                    predicate: i => i.Name.Contains(searchText),
                    include: ItemHelper.HomeDtoInclude,
                    skip: page > 1 ? page * SharedHelper.Take : null,
                    take: SharedHelper.Take
                );

            var count = await _unitOfWork.Items.GetCountAsync
                (
                    predicate: i => i.Name.Contains(searchText)
                );

            return new(data, count);
        }
        catch (Exception) { return null; }
    }

    public async Task<PageDto<HomeItemDto>?> GetItemsBySellerAsync(string sellerId, int page, string? orderBy)
    {
        try
        {
            var orderByQuery = ItemHelper.GetOrderByQuery(orderBy);
            var data = await _unitOfWork.Items.FindAllAsync
                (
                    orderBy: orderByQuery,
                    selector: ItemHelper.HomeDtoSelector,
                    predicate: i => sellerId == i.SellerId,
                    include: ItemHelper.HomeDtoInclude,
                    skip: page > 1 ? page * SharedHelper.Take : null,
                    take: SharedHelper.Take
                );

            var count = await _unitOfWork.Items.GetCountAsync
                (
                    predicate: i => sellerId == i.SellerId
                );

            return new(data, count);
        }
        catch (Exception) { return null; }
    }

    public async Task<PageDto<HomeItemDto>?> GetItemsBySubCategoriesAsync(IEnumerable<Guid> subcategoryIds, int page, string? orderBy)
    {
        try
        {
            var orderByQuery = ItemHelper.GetOrderByQuery(orderBy);
            var data = await _unitOfWork.Items.FindAllAsync
                (
                    orderBy: orderByQuery,
                    selector: ItemHelper.HomeDtoSelector,
                    predicate: i => subcategoryIds.Contains(i.SubCategoryId),
                    include: ItemHelper.HomeDtoInclude,
                    skip: page > 1 ? page * SharedHelper.Take : null,
                    take: SharedHelper.Take
                );
            
            var count = await _unitOfWork.Items.GetCountAsync
                (
                    predicate: i => subcategoryIds.Contains(i.SubCategoryId)
                );

            return new(data, count);
        }
        catch (Exception) { return null; }
    }

    public async Task<PageDto<HomeItemDto>?> GetItemsBySubCategoryAsync(Guid subcategoryId, int page, string? orderBy)
    {
        try
        {
            var orderByQuery = ItemHelper.GetOrderByQuery(orderBy);
            var data = await _unitOfWork.Items.FindAllAsync
                (
                    orderBy: orderByQuery,
                    selector: ItemHelper.HomeDtoSelector,
                    predicate: i => subcategoryId == i.SubCategoryId,
                    include: ItemHelper.HomeDtoInclude,
                    skip: page > 1 ? page * SharedHelper.Take : null,
                    take: SharedHelper.Take
                );

            var count = await _unitOfWork.Items.GetCountAsync
                (
                    predicate: i => subcategoryId == i.SubCategoryId
                );

            return new(data, count);
        }
        catch (Exception) { return null; }
    }

    public async Task<StatusDto> UnListItem(Guid id)
    {
        var item = await _unitOfWork.Items.FindAsync(
            predicate: i => i.Id == id,
            disableTracking: false);

        if (item == null)
            return new("Item not found", HttpStatusCode.NotFound);

        if (item.IsListed)
        {
            try
            {
                item.IsListed = false;
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                return new($"Item couldn't be updated.\nCause: {ex.Message}", HttpStatusCode.InternalServerError);
            }
        }

        return new("Item unlisted successfully", HttpStatusCode.NoContent);
    }
}
