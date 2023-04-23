using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedicaRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessagesManager _messagesManager;

        public MessagesController(IMessagesManager messagesManager)
        {
            _messagesManager = messagesManager;
        }

        [HttpGet("allChats")]
        public async Task<ActionResult<IEnumerable<ChatDto>>> GetUserChats(string userId, int upTo)
        {
            return Ok(await _messagesManager.GetUserChats(userId, upTo));
        }

        [HttpGet("chat")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetChat(string firstUserId, string secondUserId, DateTime dateOpened)
        {
            return Ok(await _messagesManager.GetChat(firstUserId, secondUserId, dateOpened));
        }

        [HttpGet("delete")]
        public async Task<StatusDto> DeleteMessage(string userId, Guid messageId)
        {
            return await _messagesManager.DeleteMessage(userId, messageId);
        }
    }
}
