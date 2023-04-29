using MedicaRental.BLL.Dtos;
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
        public async Task<ActionResult<IEnumerable<ChatDto>>> GetUserChats(int upTo)
        {
            var userId = _userManager.GetUserId(User);
            var chats = await _messagesManager.GetUserChats(userId, upTo);
            return Ok(chats);
        }

        [HttpGet("chat")]
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

        //[HttpDelete]
        //public async Task<StatusDto> DeleteMessage(string userId, Guid messageId)
        //{
        //    return await _messagesManager.DeleteMessage(userId, messageId);
        //}

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<StatusDto>> DeleteMessage(DeleteMessageRequestDto deleteMessageRequestDto)
        {
            var currentUserId = _userManager.GetUserId(User);
            
            var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

            if (claim?.Value == UserRoles.Client.ToString())
            {
                if (currentUserId != deleteMessageRequestDto.UserId)
                    return Unauthorized();

                return await _messagesManager.DeleteMessage(deleteMessageRequestDto.UserId, deleteMessageRequestDto.MessageId);
            }

            else 
            {

                StatusDto deleteMessageResult = await _messagesManager.DeleteMessage(deleteMessageRequestDto.UserId, deleteMessageRequestDto.MessageId);
                if (deleteMessageRequestDto.ReportId is null || deleteMessageResult.StatusCode != System.Net.HttpStatusCode.OK)
                    return StatusCode((int)deleteMessageResult.StatusCode, deleteMessageResult);

                var insertReportActionDto = new InserReportActionDto(deleteMessageResult.StatusMessage, deleteMessageRequestDto.ReportId.Value, currentUserId);
                var addingReportAction = await _reportActionManager.AddReportAction(insertReportActionDto);

                return StatusCode((int)addingReportAction.StatusCode, deleteMessageResult);
            }

        }

        [HttpGet("notificationCount")]
        public async Task<int> GetNotificationCount()
        {
            var userId = _userManager.GetUserId(User);
            return await _messagesManager.GetNotificationCount(userId);
        }

        [HttpGet("notifications")]
        public async Task<IEnumerable<MessageNotificationDto>> GetLastUnseenChats()
        {
            var userId = _userManager.GetUserId(User);
            return await _messagesManager.GetUnseenChats(userId);
        }
    }
}
