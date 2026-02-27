using CommonLib.Handles;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using WebBrowser.Services.Interfaces;

namespace WebBrowser.Services.Implements
{
    public class MyProfileService : IMyProfileService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IErrorHandler _errorHandler;

        public MyProfileService(HttpClient httpClient, IConfiguration config, IErrorHandler errorHandler)
        {
            _httpClient = httpClient;
            _config = config;
            _errorHandler = errorHandler;
        }

        private string ApiBase
        {
            get
            {
                var baseUrl = _config["ApiSettings:BaseUrl"];
                if (string.IsNullOrWhiteSpace(baseUrl))
                    baseUrl = "https://localhost:7053";
                return baseUrl.TrimEnd('/');
            }
        }

        private string MeUrl => $"{ApiBase}/api/MyproFile/me";

        public void UseToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<ApiResponse<MyProfileDto>?> GetMe()
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(MyProfileService), nameof(GetMe));

                var res = await _httpClient.GetAsync(MeUrl);
                var json = await res.Content.ReadAsStringAsync();

                // Nếu WebApi trả lỗi, vẫn deserialize được nếu nó trả JSON, còn không thì wrap lại
                if (!res.IsSuccessStatusCode)
                {
                    return new ApiResponse<MyProfileDto>
                    {
                        success = false,
                        code = ((int)res.StatusCode).ToString(),
                        message = json,
                        data = null
                    };
                }

                return JsonConvert.DeserializeObject<ApiResponse<MyProfileDto>>(json);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new ApiResponse<MyProfileDto>
                {
                    success = false,
                    code = "500",
                    message = ex.Message,
                    data = null
                };
            }
        }

        public async Task<ApiResponse<object>?> UpdateMe(UpdateMyProfileRequest model)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(MyProfileService), nameof(UpdateMe));

                var payload = JsonConvert.SerializeObject(model);
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var res = await _httpClient.PutAsync(MeUrl, content);
                var json = await res.Content.ReadAsStringAsync();

                if (!res.IsSuccessStatusCode)
                {
                    return new ApiResponse<object>
                    {
                        success = false,
                        code = ((int)res.StatusCode).ToString(),
                        message = json,
                        data = null
                    };
                }

                // API update của bạn trả { success, code, message } (không có data) -> vẫn ok deserialize object
                return JsonConvert.DeserializeObject<ApiResponse<object>>(json);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new ApiResponse<object>
                {
                    success = false,
                    code = "500",
                    message = ex.Message,
                    data = null
                };
            }
        }

        public async Task<ApiResponse<object>?> DeleteMe()
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(MyProfileService), nameof(DeleteMe));

                var res = await _httpClient.DeleteAsync(MeUrl);
                var json = await res.Content.ReadAsStringAsync();

                if (!res.IsSuccessStatusCode)
                {
                    return new ApiResponse<object>
                    {
                        success = false,
                        code = ((int)res.StatusCode).ToString(),
                        message = json,
                        data = null
                    };
                }

                return JsonConvert.DeserializeObject<ApiResponse<object>>(json);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new ApiResponse<object>
                {
                    success = false,
                    code = "500",
                    message = ex.Message,
                    data = null
                };
            }
        }
    }
}
