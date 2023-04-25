using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Managers;
using MedicaRental.DAL.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MedicaRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessagesManager _messagesManager;
        private readonly UserManager<AppUser> _userManager;

        public MessagesController(IMessagesManager messagesManager, UserManager<AppUser> userManager)
        {
            _messagesManager = messagesManager;
            _userManager = userManager;
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
            return Ok(chat);
        }

        [HttpGet("delete")]
        public async Task<StatusDto> DeleteMessage(string userId, Guid messageId)
        {
            return await _messagesManager.DeleteMessage(userId, messageId);
        }
    }
}
