using MedicaRental.DAL.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MedicaRental.API
{
    public class TestHub :Hub
    {
        private readonly UserManager<AppUser> _userManager;
        private Dictionary<string,string> users = new Dictionary<string,string>();
        public TestHub(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task InitialConnection()
        {
            users.Add(Context.UserIdentifier, Context.ConnectionId);
        }


        public async Task PostComment( string comment)
        {
            var cont = Context;
           // string user = _userManager.GetUserId(Context.User);
          //  await Console.Out.WriteLineAsync(comment);
            await Clients.Client(Context.ConnectionId).SendAsync("ReciveComment", $"{Context.User.Identity.IsAuthenticated} : {comment}");
        }
    }
}
