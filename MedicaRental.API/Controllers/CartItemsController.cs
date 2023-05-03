using MedicaRental.BLL.Managers;
using Microsoft.AspNetCore.Mvc;

namespace MedicaRental.API.Controllers;

public class CartItemsController : Controller
{
    private readonly ICartItemsManager _cartItemsManager;

    public CartItemsController(ICartItemsManager cartItemsManager)
    {
        _cartItemsManager = cartItemsManager;
    }

    public IActionResult Index()
    {
        return View();
    }
}
