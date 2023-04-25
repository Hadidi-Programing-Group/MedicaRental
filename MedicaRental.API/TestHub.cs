using MedicaRental.DAL.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MedicaRental.API
{
    public class TestHub : Hub
    {
        private readonly UserManager<AppUser> _userManager;
        
        private static Dictionary<string,string> users = new Dictionary<string,string>();
        
        public TestHub(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public override Task OnConnectedAsync()
        {

            users.TryAdd(Context.UserIdentifier, Context.ConnectionId);
            
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if(users.TryGetValue(Context.UserIdentifier, out var user))
            {
                users.Remove(Context.UserIdentifier);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public void InitialConnection()
        {
            users.Add(Context.UserIdentifier, Context.ConnectionId);
        }


        public async Task PostComment(string comment, string user)
        {
            var cont = Context;
            foreach (var item in users)
            {
                await Console.Out.WriteLineAsync($"{item.Key} : {item.Value}");
            }

            // string user = _userManager.GetUserId(Context.User);
            //  await Console.Out.WriteLineAsync(comment);
            if(users.TryGetValue(user, out string? conId))
            await Clients.Client(conId).SendAsync("ReceiveComment", $"{Context.User.Identity.IsAuthenticated} : {comment}");
        }
    }
}
