using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Report;
using MedicaRental.BLL.Managers;
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
        public async Task<ActionResult<List<ReportDtos>>> GetAllChatsReports()
        {
            List<ReportDtos> reports = (await _ReportsManager.GetChatReportsAsync()).ToList();
            if (reports == null)
                return NotFound();

            return reports;
        }

        [HttpGet]
        [Route("AllReviewReports")]
        public async Task<ActionResult<List<ReportDtos>>> GetAllReviewsReports()
        {
            List<ReportDtos> reports = (await _ReportsManager.GetReviewReportsAsync()).ToList();
            if (reports == null)
                return NotFound();

            return reports;
        }


        [HttpGet]
        [Route("AllItemsReports")]
        public async Task<ActionResult<List<ReportDtos>>> GetAllItemsReports()
        {
            List<ReportDtos> reports = (await _ReportsManager.GetItemReportsAsync()).ToList();
            if (reports == null)
                return NotFound();

            return reports;
        }


        [HttpGet]
        [Route("{Id}")]
        public async Task<ActionResult<ReportDtos>> GetById(Guid Id)
        {
            ReportDtos? report = await _ReportsManager.GetByIdAsync(Id);
            if (report is null)
                return NotFound();

            return report;
        }

        [HttpPost]
        public async Task<ActionResult> InsertReport(InsertReportDtos insertReportDtos)
        {
            InsertReportStatusDto insertreportStatusDto = await _ReportsManager.InsertNewReport(insertReportDtos);

            if (!insertreportStatusDto.isCreated)
                return BadRequest(insertreportStatusDto.StatusMessage);

            return CreatedAtAction(
                actionName: nameof(insertReportDtos),
                routeValues: new { Id = insertreportStatusDto.Id },
                value: new { Message = insertreportStatusDto.StatusMessage }
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
