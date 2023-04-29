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
        public static Dictionary<string, string> UserIds { get; } = new();
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
                UserIds.TryAdd(Context.UserIdentifier, Context.ConnectionId);

                //_messagesManager.UpdateMessageStatusToReceived(Context.UserIdentifier, DateTime.UtcNow);
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if (Context.UserIdentifier is null)
                throw new NullReferenceException(nameof(Context.UserIdentifier));

            if (UserIds.TryGetValue(Context.UserIdentifier, out var user))
            {
                UserIds.Remove(Context.UserIdentifier);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SetMessageSeen(Guid messageId, string senderId)
        {   
            await _messagesManager.UpdateMessageStatus(messageId);
            
            if (UserIds.TryGetValue(senderId, out string? conId))
            {
                await Clients.Client(conId).SendAsync("MessageSeen", messageId);
            }
        }

        public async Task AllMessagesSeen(string userId)
        {
            if (UserIds.TryGetValue(userId, out string? conId))
            {
                await Clients.Client(conId).SendAsync("AllMessagesSeen", userId);
            }
        }

        public async Task<Guid> SendMessage(string message, string receiverId, DateTime timeStamp)
        {
            var messageId = await _messagesManager.AddMessage(Context.UserIdentifier!, receiverId, message, timeStamp);
            
            if (UserIds.TryGetValue(receiverId, out string? conId))
            {
                await Clients.Client(conId).SendAsync("ReceiveMessage", messageId, message, Context!.UserIdentifier!, timeStamp.ToString("o"), MessageStatus.Sent);
            }

            return messageId;
        }
    }
}