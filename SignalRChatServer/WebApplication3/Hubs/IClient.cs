using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Hubs
{
   public interface IClient
    {
        Task ParticipantDisconnection(string name);
        Task ParticipantReconnection(string name);
        Task ParticipantLogin(User client);
        Task ParticipantLogout(string name);
        Task SendMessage(string user,string sender,string message);
        Task ParticipantTyping(string sender);
        Task SendGroupeMessage(string roomName, string user, string message);
    }
}
