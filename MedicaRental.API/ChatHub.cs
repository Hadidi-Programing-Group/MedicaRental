using MedicaRental.BLL.Managers;
using MedicaRental.DAL.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MedicaRental.API
{
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
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if(Context.UserIdentifier is null) 
                throw new NullReferenceException(nameof(Context.UserIdentifier));

            if (_userIds.TryGetValue(Context.UserIdentifier, out var user))
            {
                _userIds.Remove(Context.UserIdentifier);
            }

            return base.OnDisconnectedAsync(exception);
        }


        public async Task SendMessage(string message, string receiverId, DateTime timeStamp)
        {
            if (_userIds.TryGetValue(receiverId, out string? conId))
            {
                await Clients.Client(conId).SendAsync("ReceiveMessage", message, Context.UserIdentifier);
                await _messagesManager.AddMessage(Context.UserIdentifier!, receiverId, message, timeStamp);
            }
        }
    }
}
