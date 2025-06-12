using CoreLib.config;
using CoreLib.Dtos;
using CoreLib.Models;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using WebBrowser.Interfaces;
using WebBrowser.Models;

namespace WebBrowser.Services
{
    public class EmployeeService : IEmployeeService
    {

        private readonly string _url;
        private readonly HttpClient _client;
        public EmployeeService(IConfiguration configuration, HttpClient client)
        {
            _url = configuration["PathStrings:Url"];
            _client = client;
        }
        public async Task<ApiResponse> DeleteEmp(string manv)
        {
            var url = _url + ApiRouter.delete_emp;
            Console.WriteLine("DELETE URL: " + url);

            var payload = new { Manv = manv };
            var jsonPayload = JsonConvert.SerializeObject(payload);

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(url),
                Content = content
            };

            var response = await _client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            try
            {
                var data = System.Text.Json.JsonSerializer.Deserialize<ApiResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return data ?? new ApiResponse { Success = false, Message = "Không phân tích được phản hồi." };
            }
            catch
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Lỗi phân tích JSON: " + json
                };
            }
        }
        public async Task<ApiResponse> UpdateCommision(string manv)
        {
            var url = _url + ApiRouter.UpdateCommission;
            Console.WriteLine(url);
            var payload = new { Manv= manv};
            var jsonpayload= JsonConvert.SerializeObject(payload);
            var content= new StringContent(jsonpayload, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(url),
                Content = content
            };
            var reponse = await _client.SendAsync(request);
            var json = await reponse.Content.ReadAsStringAsync();
            try
            {
                var data = System.Text.Json.JsonSerializer.Deserialize<ApiResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return data ?? new ApiResponse { Success = false, Message = "Không phân tích được phản hồi." };
            }
            catch
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Lỗi phân tích JSON: " + json
                };
            }

        }

        public async Task<ApiResponse> Add_emp(string ho_ten, string manv, DateTime ngaysinh, string diachi,
           string phai, float luong, string mangql, int maph, DateTime ngayvao, float hoahong, string majob)
        {
            var url = _url + ApiRouter.add_emp;
            Console.WriteLine(url);

            var payload = new
            {
                HO_TEN = ho_ten,
                MANV = manv,
                NGSINH = ngaysinh,
                DCHI = diachi,
                PHAI = phai,
                LUONG = luong,
                MA_NQL = string.IsNullOrEmpty(mangql) ? null : mangql,
                MAPHG = maph,
                NGAY_VAO = ngayvao,
                HoaHong = hoahong,
                MAJOB = majob
            };

            var conten = JsonConvert.SerializeObject(payload);
            var jsondata = new StringContent(conten, Encoding.UTF8, "application/json");

            var repone = await _client.PostAsync(url, jsondata);
            var json = await repone.Content.ReadAsStringAsync();

            try
            {
                var data = System.Text.Json.JsonSerializer.Deserialize<ApiResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (data != null)
                    return data;
                else
                    return new ApiResponse { Success = false, Message = "Không thể phân tích dữ liệu." };
            }
            catch
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Lỗi phân tích JSON: " + json
                };
            }
        }

        public async Task<List<Employee>> getall()
        {
            var url = _url + ApiRouter.getall_emp;
            Console.WriteLine(url);
            var response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                // Cấu hình JsonSerializerSettings

                var setting = new JsonSerializerSettings { 
                NullValueHandling = NullValueHandling.Include,
                };
          
                return JsonConvert.DeserializeObject<List<Employee>>(json,setting);
            }

            return new List<Employee>();
        }

    }
}
