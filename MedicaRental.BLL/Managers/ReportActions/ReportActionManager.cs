using MedicaRental.BLL.Dtos;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers;

public class ReportActionManager : IReportActionManager
{
    private readonly IUnitOfWork _unitOfWork;

    public ReportActionManager(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<StatusDto> AddReportAction(InserReportActionDto insertReportActionDto)
    {
        ReportAction reportAction = new ReportAction()
        {
            Action = insertReportActionDto.Action,
            CreateTime = DateTime.Now,
            AdminId = insertReportActionDto.AdminId,    
            ReportId = insertReportActionDto.ReportId,
        };
        var insertResult = await _unitOfWork.ReportActions.AddAsync(reportAction);
        if (!insertResult)
            return new StatusDto("Failed", System.Net.HttpStatusCode.BadRequest);

        try
        {
            _unitOfWork.Save();
            return new StatusDto("Report Action Inserted", System.Net.HttpStatusCode.OK);
        }
        catch
        {
            return new StatusDto("Failed", System.Net.HttpStatusCode.InternalServerError);

        }
    }
}
