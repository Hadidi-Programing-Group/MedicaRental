using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Admin;
using MedicaRental.BLL.Dtos.Report;
using MedicaRental.BLL.Managers;
using MedicaRental.DAL.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MedicaRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportsManager _ReportsManager;
        private readonly IReportActionManager _reportActionManager;
        private readonly UserManager<AppUser> _userManager;

        public ReportsController(IReportsManager ReportsManager,
            IReportActionManager reportActionManager,
            UserManager<AppUser> userManager)
        {
            _ReportsManager = ReportsManager;
            _reportActionManager = reportActionManager;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("AllChatsReports")]
        [Authorize(Policy = ClaimRequirement.AdminPolicy)]
        public async Task<ActionResult<PageDto<ReportDto>?>> GetAllChatsReports(int page)
        {
            var reports = (await _ReportsManager.GetChatReportsAsync(page));
            if (reports == null)
                return NotFound();

            return reports;
        }

        [HttpGet]
        [Route("AllReviewReports")]
        [Authorize(Policy = ClaimRequirement.AdminPolicy)]
        public async Task<ActionResult<PageDto<ReportDto>?>> GetAllReviewsReports(int page)
        {
            var reports = (await _ReportsManager.GetReviewReportsAsync(page));
            if (reports == null)
                return NotFound();

            return reports;
        }


        [HttpGet]
        [Route("AllItemsReports")]
        [Authorize(Policy = ClaimRequirement.AdminPolicy)]
        public async Task<ActionResult<PageDto<ReportDto>?>> GetAllItemsReports(int page)
        {
            var reports = (await _ReportsManager.GetItemReportsAsync(page));
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
        [Route("MarkAsSolved/{Id}")]
        [Authorize(Policy = ClaimRequirement.AdminPolicy)]
        public async Task<ActionResult<DetailedReportDto>> MarkAsSolved(Guid Id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId is null)
                return Unauthorized();

            StatusDto markAsSolvedResult = await _ReportsManager.MarkAsSolvedAsync(Id);

            if (markAsSolvedResult.StatusCode != System.Net.HttpStatusCode.OK)
                return StatusCode((int)markAsSolvedResult.StatusCode, markAsSolvedResult);

            var insertReportActionDto = new InserReportActionDto(markAsSolvedResult.StatusMessage, Id, userId);
            var addingReportAction = await _reportActionManager.AddReportAction(insertReportActionDto);

            return StatusCode((int)addingReportAction.StatusCode, markAsSolvedResult);
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
