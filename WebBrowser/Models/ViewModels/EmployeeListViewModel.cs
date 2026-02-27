
using CoreLib.Models;

namespace WebBrowser.Models.ViewModels
{
    public class EmployeeListViewModel
    {
        public List<Employee> Employees { get; set; } = new List<Employee>();
        public ApiResponse? Response { get; set; }
    }
}
