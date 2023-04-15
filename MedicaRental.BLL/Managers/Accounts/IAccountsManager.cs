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
    Task<StatusDto> BlockUserAsync(string email, DateTime? endDate);
    Task DeleteAsync(AppUser newUser);
    Task<LoginStatusWithTokenDto> LoginAsync(LoginInfoDto loginInfoDto);
    Task<BaseUserRegisterStatusDto> RegisterNewUserAsync(BaseUserRegisterInfoDto baseUserRegisterInfoDto);
}
