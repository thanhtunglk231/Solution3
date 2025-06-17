using CoreLib.config;
using CoreLib.Dtos;
using CoreLib.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

using WebBrowser.Models;
using WebBrowser.Services.ApiServices;
using WebBrowser.Services.Interfaces;

namespace WebBrowser.Services
{
    public class EmployeeService : BaseApiService, IEmployeeService
    {

        private readonly string _url;
        private readonly HttpClient _client;
        public EmployeeService(IConfiguration configuration, HttpClient client)
           : base(configuration, client)
        {
            _url = configuration["PathStrings:Url"];
            _client = client;
        }
        public async Task<ApiResponse> DeleteEmp(string manv, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var url = _url + ApiRouter.delete_emp;
            Console.WriteLine("DELETE URL: " + url);

            var payload = new { Manv = manv };
            var result = await DeleteJsonAsync<ApiResponse>(ApiRouter.delete_emp, payload);
            return result ?? new ApiResponse { Success = false, Message = "Không phân tích được phản hồi." };
        }
        public async Task<ApiResponse> UpdateSalary(string manv, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var url = _url + ApiRouter.UpdateSalary;
            Console.WriteLine(url);
            var payload = new { Manv = manv };
            var result = await PutJsonAsync<ApiResponse>(ApiRouter.UpdateSalary, payload);
            return result ?? new ApiResponse { Success = false, Message = "Không phân tích được phản hồi." };

        }


        public async Task<ApiResponse> UpdateCommision(string manv, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var url = _url + ApiRouter.UpdateCommission;
            Console.WriteLine(url);
            var payload = new { Manv = manv };
            var result = await PutJsonAsync<ApiResponse>(ApiRouter.UpdateCommission, payload);
            return result ?? new ApiResponse { Success = false, Message = "Không phân tích được phản hồi." };

        }

        public async Task<ApiResponse> Add_emp(string ho_ten, string manv, DateTime ngaysinh, string diachi,
           string phai, float luong, string mangql, int maph, DateTime ngayvao, float hoahong, string majob, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var url = _url + ApiRouter.add_emp;
            Console.WriteLine(url);

            var employee = new Employee
            {
                HO_TEN = ho_ten?.Trim(),
                MANV = manv?.Trim(),
                NGSINH = ngaysinh,
                DCHI = diachi?.Trim(),
                PHAI = phai?.Trim(),
                LUONG = luong,
                MA_NQL = string.IsNullOrEmpty(mangql) ? null : mangql.Trim(),
                MAPHG = maph,
                NGAY_VAO = ngayvao,
                HOAHONG = hoahong,
                MAJOB = majob?.Trim()
            };

            var result = await PostJsonAsync<ApiResponse>(ApiRouter.add_emp, employee);
            return result ?? new ApiResponse { Success = false, Message = "Không phân tích được phản hồi." };
        }
        public async Task<List<HistoryDto>> GetHistory(string manv, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var url = $"{_url.TrimEnd('/')}/Employee/HisEmp?manv={manv}";
            var response = await _client.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            Console.WriteLine("JSON nhận được từ API:");
            Console.WriteLine(json);

            if (!response.IsSuccessStatusCode)
                return new List<HistoryDto>();

            try
            {
                var jObj = JObject.Parse(json);
                var dataJson = jObj["data"]?.ToString(); 

                if (string.IsNullOrEmpty(dataJson))
                    return new List<HistoryDto>();

                var data = JsonConvert.DeserializeObject<List<HistoryDto>>(dataJson);
                return data ?? new List<HistoryDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi phân tích JSON: " + ex.Message);
                return new List<HistoryDto>();
            }
        }



        public async Task<List<Employee>> getall(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var url = _url + ApiRouter.getall_emp;
            Console.WriteLine(url);
            var response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                // Cấu hình JsonSerializerSettings

                var setting = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include,
                };

                return JsonConvert.DeserializeObject<List<Employee>>(json, setting);
            }

            return new List<Employee>();
        }

    }
}