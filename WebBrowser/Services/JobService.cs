using CoreLib.config;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using WebBrowser.Models;
using WebBrowser.Interfaces;

namespace WebBrowser.Services
{
    public class JobService : IJobService
    {
        private readonly string _url;
        private readonly HttpClient _client;

        public JobService(IConfiguration configuration, HttpClient client)
        {
            _client = client;
            _url = configuration["PathStrings:Url"] +"Job" +ApiRouter.Addjob ;
        }

        public async Task<ApiResponse> Addjob(string id, string jobname)
        {
            var requestUrl = $"{_url}/?id={id}&jobName={jobname}";
            Console.WriteLine(requestUrl);
            var response = await _client.PostAsync(requestUrl, null);

            var json = await response.Content.ReadAsStringAsync();
            Console.Write(json);

            try
            {
                var data = JsonSerializer.Deserialize<ApiResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return data ?? new ApiResponse { Success = false, Message = "Không thể phân tích dữ liệu." };
            }
            catch
            {
                return new ApiResponse { Success = false, Message = "Lỗi phân tích JSON: " + json };
            }
        }
    }
}
