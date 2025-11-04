namespace WebBrowser.Models
{
    public class TotpEnrollStartResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string secretBase32 { get; set; }
        public string otpauthUrl { get; set; }
        public string qrPngBase64 { get; set; }
    }
}
