using Newtonsoft.Json;

namespace WebBrowser.Models
{
    public class Department
    {
        [JsonProperty("maphg")]
        public int MAPHG { get; set; }

        [JsonProperty("tenphg")]
        public string TENPHG { get; set; }

        [JsonProperty("trphg")]
        public string TRPHG { get; set; }

        [JsonProperty("nG_NHANCHUC")]
        public DateTime? NG_NHANCHUC { get; set; }
    }
}
