using MedicaRental.BLL.Dtos;
using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers;

public interface IClientsManager
{
    Task<StatusDto> ApproveUserAsync(string email);
    Task<UserApprovalInfoDto?> GetClientApprovalInfoAsync(string userId);
    Task<UserProfileInfoDto?> GetClientInfoAsync(string userId);
    Task<ClientRegisterStatusDto> RegisterNewUserAsync(ClientRegisterInfoDto registerInfoDto);
    Task<StatusDto> UpdateApprovalInfoAsync(string userId, UpdateApprovalInfoDto updateProfileInfoDto);
    Task<StatusDto> UpdateClientInfoAsync(string userId, UpdateProfileInfoDto updateProfileInfoDto);

    Task<List<UserApprovalInfoDto>> GetAllClientsApprovalInfoAsync();
    Task<List<UserProfileInfoDto>> GetAllClientsAsync();

    Task<IEnumerable<UserProfileInfoWithIdDto>> GetClientsNeedingApprovalAsync();
}
