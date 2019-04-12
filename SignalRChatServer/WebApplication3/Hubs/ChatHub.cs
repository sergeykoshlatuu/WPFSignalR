using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Hubs
{
    public class ChatHub : Hub<IClient>
    {
        private static ConcurrentDictionary<string, User> ChatClients = new ConcurrentDictionary<string, User>();

        private List<string> groups = new List<string>() { "AllUser", "Info", "Flud" };

        public async Task Send(string user, string sender, string message)
        {
            
            if (!string.IsNullOrEmpty(sender) && user != sender &&
                !string.IsNullOrEmpty(message) && ChatClients.ContainsKey(user))
            {
                User client = new User();
                ChatClients.TryGetValue(user, out client);
               await Clients.Client(client.ID).SendMessage(sender,sender, message);
            }
        }



        public List<string> JoinRoom()
        {
            for (int i = 0; i < groups.Count; i++)
            {
                Groups.AddToGroupAsync(Context.ConnectionId, groups[i]);
            }
            return groups;
        }

        public Task LeaveRoom(string roomName)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }

        public async Task SendToGroup(string roomName, string user, string message)
        {
            await Clients.Group(roomName).SendGroupeMessage(roomName, user, message);
        }

        public List<User> Login(string name)
        {
            if (!ChatClients.ContainsKey(name))
            {
                Console.WriteLine($"++ {name} logged in");
                List<User> users = new List<User>(ChatClients.Values);
                User newUser = new User { Name = name, ID = Context.ConnectionId, IsLogedIn=true};
                var added = ChatClients.TryAdd(name, newUser);
                if (!added) return null;
                //users = new List<User>(ChatClients.Values);
                Clients.Others.ParticipantLogin(newUser);
                return users;
            }
            return null;
        }

        public void Logout()
        {
            var name = ChatClients.SingleOrDefault((c) => c.Value.ID == Context.ConnectionId).Key;
            if (!string.IsNullOrEmpty(name))
            {
                User client = new User();
                ChatClients.TryRemove(name, out client);
                Clients.Others.ParticipantLogout(name);
                client.IsLogedIn = false;
                ChatClients.TryUpdate(name, client, new User());
                Console.WriteLine($"-- {name} logged out");
            }
            
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var userName = ChatClients.SingleOrDefault((c) => c.Value.ID == Context.ConnectionId).Key;
            if (userName != null)
            {
                Clients.Others.ParticipantDisconnection(userName);
                Console.WriteLine($"<> {userName} disconnected");
            }
            return base.OnDisconnectedAsync(exception);
        }

        public Task Typing(string recepient)
        {
            if (string.IsNullOrEmpty(recepient)) return null;
            List<User> users = new List<User>(ChatClients.Values);
            var sender = users.Where((u) => u.ID == Context.ConnectionId).FirstOrDefault();
            User client = new User();
            ChatClients.TryGetValue(recepient, out client);
            Clients.Client(client.ID).ParticipantTyping(sender.Name);
            return null;
        }
    }
}
