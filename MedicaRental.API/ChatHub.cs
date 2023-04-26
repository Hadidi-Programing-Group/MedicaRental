using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Admin;
using MedicaRental.BLL.Managers;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MedicaRental.API
{
    [Authorize]
    public class ChatHub : Hub
    {
        private static Dictionary<string, string> _userIds = new Dictionary<string, string>();
        private readonly IMessagesManager _messagesManager;

        public ChatHub(IMessagesManager messagesManager)
        {
            _messagesManager = messagesManager;
        }

        public override Task OnConnectedAsync()
        {
            //set all messages sent to him as received and setup notifications

            if (Context.UserIdentifier is not null)
            {
                _userIds.TryAdd(Context.UserIdentifier, Context.ConnectionId);

                //_messagesManager.UpdateMessageStatusToReceived(Context.UserIdentifier, DateTime.Now);
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if (Context.UserIdentifier is null)
                throw new NullReferenceException(nameof(Context.UserIdentifier));

            if (_userIds.TryGetValue(Context.UserIdentifier, out var user))
            {
                _userIds.Remove(Context.UserIdentifier);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SetMessageSeen(Guid messageId, string senderId)
        {   
            await _messagesManager.UpdateMessageStatus(messageId);
            
            if (_userIds.TryGetValue(senderId, out string? conId))
            {
                await Clients.Client(conId).SendAsync("MessageSeen", messageId);
            }
        }

        public async Task<Guid> SendMessage(string message, string receiverId, DateTime timeStamp)
        {
            var messageId = await _messagesManager.AddMessage(Context.UserIdentifier!, receiverId, message, timeStamp);
            
            if (_userIds.TryGetValue(receiverId, out string? conId))
            {
                await Clients.Client(conId).SendAsync("ReceiveMessage", messageId, message, Context!.UserIdentifier!, timeStamp, MessageStatus.Sent);
            }

            return messageId;
        }
    }
}