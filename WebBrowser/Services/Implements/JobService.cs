using CoreLib.config;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using WebBrowser.Models;

using CoreLib.Models;
using Newtonsoft.Json;
using WebBrowser.Services.Interfaces;
using WebBrowser.Services.ApiServices;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace WebBrowser.Services
{
    public class JobService : BaseApiService, IJobService
    {
        private readonly string _baseUrl;
        private readonly HttpClient _client;

        public JobService(IConfiguration configuration, HttpClient client)
        : base(configuration, client)
        {
            _client = client;
            _baseUrl = configuration["PathStrings:Url"];
        }

        public async Task<List<Job>> getall(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var result = await GetAsync<List<Job>>(ApiRouter.get_all_job);

            return result ?? new List<Job>();
        }
        public async Task<ApiResponse> DeleteJob(string majob, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var result = await DeleteAsync<ApiResponse>($"{ApiRouter.del_job}?majob={majob}");

            return result ?? new ApiResponse { Success = false, Message = "Không nhận được phản hồi." };
        }


        public async Task<ApiResponse> Addjob(string id, string jobname, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var requestUrl = $"{ApiRouter.Addjob}?id={id}&jobName={jobname}";

            var dataList = await PostAsync<List<ApiResponse>>(requestUrl);

            var data = dataList?.FirstOrDefault();
            return data ?? new ApiResponse { Success = false, Message = "Không có dữ liệu từ API." };
        }

    }

}