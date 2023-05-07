using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Account;
using MedicaRental.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers;

public interface IAccountsManager
{
    Task<StatusDto> BlockUserAsync(BlockUserInfoDto blockUserInfo);
    Task DeleteAsync(AppUser newUser);
    Task<StatusDto> ForgetPassword(ForgotPasswordDto forgotPasswordDto);
    Task<LoginStatusWithTokenDto> LoginAsync(LoginInfoDto loginInfoDto);
    Task<BaseUserRegisterStatusDto> RegisterNewUserAsync(BaseUserRegisterInfoDto baseUserRegisterInfoDto);
    Task<StatusDto> ResetPassword(ResetPasswordDto resetPasswordDto);
    Task<StatusDto> UnBlockUserAsync(string email);
}
