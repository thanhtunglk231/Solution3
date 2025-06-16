namespace WebBrowser.Models
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<Dictionary<string, object>>? Data { get; set; }
    }
}
