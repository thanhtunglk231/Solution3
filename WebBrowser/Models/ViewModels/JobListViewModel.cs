using CoreLib.Models;

namespace WebBrowser.Models.ViewModels
{
    public class JobListViewModel
    {
        public List<Job> Jobs { get; set; } = new();
        public ApiResponse? Response { get; set; }
    }
}