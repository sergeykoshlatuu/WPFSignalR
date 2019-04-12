using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Services
{
   public interface IChatService
    {
        event Action<User> ParticipantLoggedIn;
        event Action<string> ParticipantLoggedOut;
        event Action<string> ParticipantDisconnected;
        event Action<string> ParticipantReconnected;
        event Action<string, string, string> SendMessage;
        event Action<string, string, string> AddMessageToGroup;
        event Action<string> ParticipantTyping;

        Task ConnectAsync();
        Task<List<User>> LoginAsync(string name);
        Task<List<string>> JoinRooms();
        Task LogoutAsync();

        Task SendMessageAsync(string name, string sender, string msg);
        Task SendMessageToGroupAsync(string roomName, string name, string msg);
        Task JoinToGroop(string roomName);
        Task TypingAsync(string recepient);
    }
}
