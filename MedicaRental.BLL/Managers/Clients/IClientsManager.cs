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
    Task<ClientRegisterStatusDto> RegisterNewUserAsync(ClientRegisterInfoDto registerInfoDto);
}
