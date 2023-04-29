using MedicaRental.BLL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers;

public interface IReportActionManager
{
    Task<StatusDto> AddReportAction(InserReportActionDto insertReportActionDto);
}
