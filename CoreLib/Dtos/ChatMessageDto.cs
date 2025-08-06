using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Dtos
{
    public class ChatMessageDto
    {
        public string SenderUsername { get; set; }
        public string ReceiverUsername { get; set; }
        public string GroupId { get; set; }
        public string MessageText { get; set; }  // 👈 đúng tên với SP
    }


}
