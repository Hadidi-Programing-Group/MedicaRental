using MedicaRental.BLL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers;

public interface ICartItemsManager
{
    Task<StatusDto> AddToCartAsync(AddToCartRequestDto addToCartRequest, string userId);
    Task<StatusDto> RemoveCartItemAsync(Guid itemId, string userId);
}
