using MedicaRental.BLL.Dtos;
using MedicaRental.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers;

public interface IAccountsManager
{
    Task DeleteAsync(AppUser newUser);
    Task<BaseUserRegisterStatusDto> RegisterNewUserAsync(BaseUserRegisterInfoDto baseUserRegisterInfoDto);
}
