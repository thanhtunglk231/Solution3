using Newtonsoft.Json;

namespace WebBrowser.Models
{
    public class TableWrapper<T>
    {
        [JsonProperty("table")]
        public List<T> Table { get; set; }
    }
}
