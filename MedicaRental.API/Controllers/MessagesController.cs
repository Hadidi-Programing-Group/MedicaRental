using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Admin;
using MedicaRental.BLL.Dtos.Message;
using MedicaRental.BLL.Managers;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Net;
using System.Security.Claims;

namespace MedicaRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessagesManager _messagesManager;
        private readonly IReportActionManager _reportActionManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHubContext<ChatHub> _chatHub;

        public MessagesController(IMessagesManager messagesManager,
            IReportActionManager reportActionManager,
            UserManager<AppUser> userManager, IHubContext<ChatHub> chatHub)
        {
            _messagesManager = messagesManager;
            _reportActionManager = reportActionManager;
            _userManager = userManager;
            _chatHub = chatHub;
        }

        [HttpGet("allChats")]
        [Authorize(Policy = ClaimRequirement.ClientPolicy)]
        public async Task<ActionResult<IEnumerable<ChatDto>>> GetUserChats(int upTo)
        {
            var userId = _userManager.GetUserId(User);
            var chats = await _messagesManager.GetUserChats(userId, upTo);
            return Ok(chats);
        }

        [HttpGet("chat")]
        [Authorize(Policy = ClaimRequirement.ClientPolicy)]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetChat(string secondUserId, int upTo, DateTime dateOpened)
        {
            var userId = _userManager.GetUserId(User);
            var chat = await _messagesManager.GetChat(userId, secondUserId, upTo, dateOpened);

            if (ChatHub.UserIds.TryGetValue(secondUserId, out string? conId))
            {
                await _chatHub.Clients.Client(conId).SendAsync("AllMessagesSeen", userId);
            }

            return Ok(chat);
        }

        [HttpDelete("{messageId}")]
        [Authorize(Policy = ClaimRequirement.ClientPolicy)]
        public async Task<ActionResult<StatusDto>> DeleteMessage(Guid messageId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var result =  await _messagesManager.DeleteMessage(messageId, currentUserId);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Policy = ClaimRequirement.ModeratorPolicy)]  
        public async Task<ActionResult<StatusDto>> DeleteMessageSuper(DeleteMessageRequestDto deleteMessageRequestDto)
        {
            StatusDto deleteMessageResult = await _messagesManager.DeleteMessage(deleteMessageRequestDto.MessageId);
            
            if (deleteMessageRequestDto.ReportId is null || deleteMessageResult.StatusCode != HttpStatusCode.OK)
                return StatusCode((int)deleteMessageResult.StatusCode, deleteMessageResult);

            var currentUserId = _userManager.GetUserId(User);
            var insertReportActionDto = new InserReportActionDto(deleteMessageResult.StatusMessage, deleteMessageRequestDto.ReportId.Value, currentUserId);
            var addingReportAction = await _reportActionManager.AddReportAction(insertReportActionDto);

            return StatusCode((int)addingReportAction.StatusCode, deleteMessageResult);
        }

        [HttpGet("notificationCount")]
        [Authorize]
        public async Task<int> GetNotificationCount()
        {
            var userId = _userManager.GetUserId(User);
            return await _messagesManager.GetNotificationCount(userId);
        }

        [HttpGet("notifications")]
        [Authorize(Policy = ClaimRequirement.ClientPolicy)]
        public async Task<IEnumerable<MessageNotificationDto>> GetLastUnseenChats()
        {
            var userId = _userManager.GetUserId(User);
            return await _messagesManager.GetUnseenChats(userId);
        }
    }
}
