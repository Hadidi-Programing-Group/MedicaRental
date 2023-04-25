using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Admin;
using MedicaRental.BLL.Dtos.Report;
using MedicaRental.BLL.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedicaRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportsManager _ReportsManager;
        public ReportsController(IReportsManager ReportsManager)
        {
            _ReportsManager = ReportsManager;
        }

        [HttpGet]
        [Route("AllChatsReports")]
        [Authorize(Policy = ClaimRequirement.AdminPolicy)]
        public async Task<ActionResult<List<ReportDto>>> GetAllChatsReports()
        {
            List<ReportDto> reports = (await _ReportsManager.GetChatReportsAsync()).ToList();
            if (reports == null)
                return NotFound();

            return reports;
        }

        [HttpGet]
        [Route("AllReviewReports")]
        [Authorize(Policy = ClaimRequirement.AdminPolicy)]
        public async Task<ActionResult<List<ReportDto>>> GetAllReviewsReports()
        {
            List<ReportDto> reports = (await _ReportsManager.GetReviewReportsAsync()).ToList();
            if (reports == null)
                return NotFound();

            return reports;
        }


        [HttpGet]
        [Route("AllItemsReports")]
        [Authorize(Policy = ClaimRequirement.AdminPolicy)]
        public async Task<ActionResult<List<ReportDto>>> GetAllItemsReports()
        {
            List<ReportDto> reports = (await _ReportsManager.GetItemReportsAsync()).ToList();
            if (reports == null)
                return NotFound();

            return reports;
        }


        [HttpGet]
        [Route("{Id}")]
        public async Task<ActionResult<DetailedReportDto>> GetById(Guid Id)
        {
            DetailedReportDto? report = await _ReportsManager.GetByIdAsync(Id);
            if (report is null)
                return NotFound();
            return report;
        }

        [HttpPost]
        [Route("InsertReport")]
        public async Task<ActionResult> InsertReport(InsertReportDtos insertReportDtos)
        {
            InsertReportStatusDto insertReportStatusDto = await _ReportsManager.InsertNewReport(insertReportDtos);

            if (!insertReportStatusDto.isCreated)
                return BadRequest(insertReportStatusDto.StatusMessage);

            return CreatedAtAction(
                actionName: "GetById", 
                routeValues: new { id = insertReportStatusDto.Id }, 
                value: new { Message = insertReportStatusDto.StatusMessage }
            );
        }


        [HttpDelete]
        [Route("{Id}")]
        public async Task<ActionResult> DeleteReportAsync(Guid Id)
        {
            DeleteReportStatusDto deleteReportStatusDto = await _ReportsManager.DeleteByIdAsync(Id);

            if (!deleteReportStatusDto.isDeleted)
                return BadRequest(deleteReportStatusDto.StatusMessage);

            return NoContent();
        }

    }
}
