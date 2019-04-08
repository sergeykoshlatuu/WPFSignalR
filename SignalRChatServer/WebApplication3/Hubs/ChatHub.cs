using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace WebApplication3.Hubs
{
    public class ChatHub : Hub
    {
        public async Task Send(string user, string message)
        {
            await Clients.All.SendAsync("AddMessage", user, message);
        }
    }
}
