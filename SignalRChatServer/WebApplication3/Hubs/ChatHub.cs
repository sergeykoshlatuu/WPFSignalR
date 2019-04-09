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

        public Task JoinRoom(string roomName)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }

        public Task LeaveRoom(string roomName)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }

        public async Task SendToGroup(string roomName,string user, string message)
        {
            await Clients.Group(roomName).SendAsync("AddMessageToGroup",roomName, user, message);
        }
    }
}
