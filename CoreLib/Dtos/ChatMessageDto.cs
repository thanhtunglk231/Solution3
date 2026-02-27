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
            public string MessageText { get; set; } // Nội dung tin nhắn

            // 1 file
            public string AttachmentUrl { get; set; }

            // Nhiều file
            public List<string> AttachmentUrls { get; set; } = new();

            public DateTime? Timestamp { get; set; } = DateTime.UtcNow;
        
    }

}



