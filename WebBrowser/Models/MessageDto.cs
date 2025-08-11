using Newtonsoft.Json;

namespace WebBrowser.Models
{
    

        public class MessageDto
        {
            [JsonProperty("sendeR_USERNAME")]
            public string SenderUsername { get; set; }

            [JsonProperty("receiveR_USERNAME")]
            public string ReceiverUsername { get; set; }

            [JsonProperty("messagE_TEXT")]
            public string MessageText { get; set; }

            [JsonProperty("senT_AT")]
            public DateTime SentAt { get; set; }

            [JsonProperty("groupId")]
            public string GroupId { get; set; }

        [JsonProperty("attachmenT_URL")] // thêm cột attachment_url
        public string AttachmentUrl { get; set; }

    }
}
