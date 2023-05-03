using MedicaRental.BLL.Dtos;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers;

public class CartItemsManager : ICartItemsManager
{
    private readonly IUnitOfWork _unitOfWork;

    public CartItemsManager(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<StatusDto> AddToCartAsync(AddToCartRequestDto addToCartRequest, string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return new StatusDto("Invalid User Id", System.Net.HttpStatusCode.Unauthorized);

        var client = await _unitOfWork.Clients.FindAsync(c => c.Id == userId);
        if (client is null)
            return new StatusDto("User is not found", System.Net.HttpStatusCode.NotFound);

        var item = await _unitOfWork.Items.FindAsync(i => i.Id == addToCartRequest.ItemId);
        if (item is null)
            return new StatusDto("Item is not found", System.Net.HttpStatusCode.NotFound);

        if (item.SellerId != client.Id)
            return new StatusDto("Item is not yours", System.Net.HttpStatusCode.Forbidden);

        var cartItem = new CartItem()
        {
            ItemId = item.Id,
            ClientId = client.Id,
        };


        try
        {
            await _unitOfWork.CartItems.AddAsync(cartItem);
            _unitOfWork.Save();
            return new StatusDto("Item added to cart", System.Net.HttpStatusCode.Created);
        }
        catch
        {
            return new StatusDto("Could't add item to cart", System.Net.HttpStatusCode.InternalServerError);
        }
    }
}
