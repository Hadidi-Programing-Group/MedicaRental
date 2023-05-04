using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Admin;
using MedicaRental.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers;

public interface IAdminsManager
{
    Task<UserRoleUpdateStatusDto> UpdateUserRoleAsync(UserRoleUpdateDto userRoleUpdateDto);

    Task<ReportUpdateStatusDto> UpdateReportStatus(ReportUpdateDto updateReportDto);


    Task<IEnumerable<RoleMangerUserInfoDto>> GetAllAdminMod();

    Task<StatusDto> DeleteAdminMod(string id); 
}
