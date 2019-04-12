using ChatClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Models
{
   public class Group : ViewModelBase
    {
        public string Name { get; set; }
        
        public ObservableCollection<ChatMessage> Chatter { get; set; }

        private bool _hasSentNewMessage;
        public bool HasSentNewMessage
        {
            get { return _hasSentNewMessage; }
            set { _hasSentNewMessage = value; OnPropertyChanged(); }
        }

        public Group() { Chatter = new ObservableCollection<ChatMessage>(); }
    }
}