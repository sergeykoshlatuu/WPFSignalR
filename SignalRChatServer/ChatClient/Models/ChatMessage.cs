using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Models
{
    public class ChatMessage
    {
        public string Message { get; set; }
        public string Author { get; set; }
        public string GroupName { get; set; }
        public DateTime Time { get; set; }
        public bool IsOriginMaster { get; set; }

        public override string ToString()
        {
            return Author + ":  " + Message;
        }
    }
}
