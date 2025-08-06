using Newtonsoft.Json;

public class GroupMessageModel
{
    [JsonProperty("messagE_ID")]
    public string MessageId { get; set; }

    [JsonProperty("grouP_ID")]
    public string GroupId { get; set; }

    [JsonProperty("sendeR_USERNAME")]
    public string SenderUsername { get; set; }

    [JsonProperty("sendeR_EMAIL")]
    public string SenderEmail { get; set; }

    [JsonProperty("messagE_TEXT")]
    public string MessageText { get; set; }

    [JsonProperty("senT_AT")]
    public DateTime SentAt { get; set; }
}
