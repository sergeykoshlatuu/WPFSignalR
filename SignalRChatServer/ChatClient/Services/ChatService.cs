using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Services
{
    public class ChatService : IChatService
    {
        public event Action<string> ParticipantDisconnected;
        public event Action<User> ParticipantLoggedIn;
        public event Action<string> ParticipantLoggedOut;
        public event Action<string> ParticipantReconnected;
        public event Action<string,string, string> SendMessage;
        public event Action<string,string, string> AddMessageToGroup;
        public event Action<string> ParticipantTyping;

        const string ServerURI = "https://localhost:44334//chathub";
        public HubConnection Connection { get; set; }
        public async Task ConnectAsync()
        {
            Connection = new HubConnectionBuilder()
        .WithUrl(ServerURI)
        .Build();
            Connection.On<string,string, string>("SendMessage", (name,sender, message) =>
               SendMessage?.Invoke(name,sender, message));
            Connection.On<string, string, string>("SendGroupeMessage", (roomName, name, message) =>
             AddMessageToGroup?.Invoke(roomName, name, message));
            Connection.On<User>("ParticipantLogin", (u) => ParticipantLoggedIn?.Invoke(u));
            Connection.On<string>("ParticipantLogout", (n) => ParticipantLoggedOut?.Invoke(n));
            Connection.On<string>("ParticipantDisconnection", (n) => ParticipantDisconnected?.Invoke(n));
            Connection.On<string>("ParticipantReconnection", (n) => ParticipantReconnected?.Invoke(n));
            Connection.On<string>("ParticipantTyping", (p) => ParticipantTyping?.Invoke(p));

            await Connection.StartAsync();
        }

        public async Task<List<User>> LoginAsync(string name)
        {
            return await Connection.InvokeAsync<List<User>>("Login",name );
        }

        public async Task<List<string>> JoinRooms()
        {
            return await Connection.InvokeAsync<List<string>>("JoinRoom");
        }

        public async Task LogoutAsync()
        {
            await Connection.InvokeAsync("Logout");
        }


        public async Task SendMessageAsync(string name,string sender, string msg)
        {
            await Connection.InvokeAsync("Send", name,sender, msg);
        }

        public async Task SendMessageToGroupAsync(string roomName, string name, string msg)
        {
           await Connection.InvokeAsync("SendToGroup", roomName, name, msg);
        }

        public async Task JoinToGroop(string roomName)
        {
            await Connection.InvokeAsync("JoinRoom", roomName);
        }

        public async  Task TypingAsync(string recepient)
        {
            await Connection.InvokeAsync("Typing", recepient);
        }
    }
}
