using CoreLib.config;
using CoreLib.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebBrowser.Interfaces;

namespace WebBrowser.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly string _url;
        private readonly HttpClient _client;
        public DepartmentService(IConfiguration configuration, HttpClient client)
        {
            _url = configuration["PathStrings:Url"] + ApiRouter.GetDeptbyid;
            _client = client;
        }
        public async Task<List<Department>> GetDeptbyid(int id)
        {
            var reponse = await _client.GetAsync(_url + id);
            if (reponse.IsSuccessStatusCode)
            {
                var json = await reponse.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Department>>(json);
            }
            return new List<Department>();
        }
    }
}
