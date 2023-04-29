using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Message;
using MedicaRental.BLL.Managers;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace MedicaRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessagesManager _messagesManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHubContext<ChatHub> _chatHub;

        public MessagesController(IMessagesManager messagesManager, UserManager<AppUser> userManager, IHubContext<ChatHub> chatHub)
        {
            _messagesManager = messagesManager;
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

        [HttpDelete]
        public async Task<StatusDto> DeleteMessage(string userId, Guid messageId)
        {
            return await _messagesManager.DeleteMessage(userId, messageId);
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
