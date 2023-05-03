using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Admin;
using MedicaRental.BLL.Managers;
using MedicaRental.DAL.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MedicaRental.API.Controllers;

public class CartItemsController : Controller
{
    private readonly ICartItemsManager _cartItemsManager;
    private readonly UserManager<AppUser> _userManager;

    public CartItemsController(ICartItemsManager cartItemsManager, UserManager<AppUser> userManager)
    {
        _cartItemsManager = cartItemsManager;
        _userManager = userManager;
    }

    [Authorize(Policy = ClaimRequirement.ClientPolicy)]
    public async Task<ActionResult<StatusDto>> AddToCart(AddToCartRequestDto addToCartRequest)
    {
        var userId = _userManager.GetUserId(User);
        StatusDto addToCartResult = await _cartItemsManager.AddToCartAsync(addToCartRequest, userId);
        return StatusCode((int)addToCartResult.StatusCode, addToCartResult);
    }
}
