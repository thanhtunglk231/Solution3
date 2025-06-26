using CommonLib.Handles;
using CoreLib.Models;
using Newtonsoft.Json;
using System.Data;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using WebBrowser.Models;

namespace WebBrowser.Services.ApiServices
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _client;
        private readonly IErrorHandler _errorHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpService(IHttpClientFactory httpClientFactory, IConfiguration config, IErrorHandler errorHandler, IHttpContextAccessor httpContextAccessor)
        {
            _errorHandler = errorHandler;
            _httpContextAccessor = httpContextAccessor;
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri(config["PathStrings:Url"]);
        }

        private void AddBearerToken()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        private StringContent CreateJsonContent(object data)
        {
            var json = JsonConvert.SerializeObject(data);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        public async Task<DataTable> GetDataTableAsync(string url)
        {
            _errorHandler.WriteStringToFuncion("HttpService", nameof(GetDataTableAsync));
            AddBearerToken();

            var ds = await GetAsync<DataSet>(url);
            return ds?.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
        }

        public async Task<DataRow> GetDataRowAsync(string url)
        {
            _errorHandler.WriteStringToFuncion("HttpService", nameof(GetDataRowAsync));
            var dt = await GetDataTableAsync(url);
            return dt.Rows.Count > 0 ? dt.Rows[0] : dt.NewRow();
        }

        public async Task<CResponseMessage1> GetResponseAsync(string url)
        {
            _errorHandler.WriteStringToFuncion("HttpService", nameof(GetResponseAsync));
            return await GetAsync<CResponseMessage1>(url);
        }

        public async Task<DataTable> PostDataTableAsync(string url, object data)
        {
            _errorHandler.WriteStringToFuncion("HttpService", nameof(PostDataTableAsync));
            _errorHandler.WriteStringToFile(nameof(PostDataTableAsync), data);
            AddBearerToken();

            var ds = await PostAsync<DataSet>(url, data);
            return ds?.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
        }

        public async Task<DataRow> PostDataRowAsync(string url, object data)
        {
            _errorHandler.WriteStringToFuncion("HttpService", nameof(PostDataRowAsync));
            _errorHandler.WriteStringToFile(nameof(PostDataRowAsync), data);

            var dt = await PostDataTableAsync(url, data);
            return dt.Rows.Count > 0 ? dt.Rows[0] : dt.NewRow();
        }

        public async Task<CResponseMessage1> PostResponseAsync(string url, object data)
        {
            _errorHandler.WriteStringToFuncion("HttpService", nameof(PostResponseAsync));
            _errorHandler.WriteStringToFile(nameof(PostResponseAsync), data);
            AddBearerToken();

            return await PostAsync<CResponseMessage1>(url, data);
        }

        private async Task<T> GetAsync<T>(string url)
        {
            _errorHandler.WriteStringToFuncion("HttpService", nameof(GetAsync));
            AddBearerToken();

            try
            {
                var response = await _client.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Lỗi API: {response.StatusCode} - {json}");
                }

                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                throw;
            }
        }
        public async Task<T> PostAsync<T>(string url, object data)
        {
            var startTime = DateTime.Now;

            try
            {
                _errorHandler.WriteStringToFuncion(nameof(PostAsync), url);
                _errorHandler.WriteStringToFile("POST Data", data);
                AddBearerToken();

                var jsonContent = CreateJsonContent(data);
                var response = await _client.PostAsync(url, jsonContent);

                var endTime = DateTime.Now;
          

                _errorHandler.WriteStringToFile("Response Status", response.StatusCode.ToString());
              

                var json = await response.Content.ReadAsStringAsync();
                _errorHandler.WriteStringToFile("Response Content", json);

                // Nếu API trả về object chứa "result"
                if (typeof(T) == typeof(CResponseMessage1))
                {
                    var rootObj = JsonConvert.DeserializeObject<dynamic>(json);
                    var resultToken = rootObj?.result;

                    if (resultToken != null)
                    {
                        var resultStr = JsonConvert.SerializeObject(resultToken);
                        var resultObj = JsonConvert.DeserializeObject<CResponseMessage1>(resultStr);

                        resultObj.code ??= ((int)response.StatusCode).ToString();
                        resultObj.Success = response.IsSuccessStatusCode;
                        return (T)(object)resultObj;
                    }

                    return (T)(object)new CResponseMessage1
                    {
                        Success = false,
                        code = ((int)response.StatusCode).ToString(),
                        message = "Phản hồi không chứa result"
                    };
                }

           
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Lỗi gọi API: {response.StatusCode} - {json}");
                }

                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);

                if (typeof(T) == typeof(CResponseMessage1))
                {
                    return (T)(object)new CResponseMessage1
                    {
                        Success = false,
                        code = "500",
                        message = "Lỗi hệ thống: " + ex.Message
                    };
                }

                return default;
            }
        }


        public async Task<string> GetRawJsonAsync(string url)
        {
            _errorHandler.WriteStringToFuncion("HttpService", nameof(GetRawJsonAsync));
            AddBearerToken();

            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<List<T>> GetListAsync<T>(string url)
        {
            _errorHandler.WriteStringToFuncion("HttpService", nameof(GetListAsync));
            return await GetAsync<List<T>>(url);
        }

        public async Task<DataSet> GetDataSetFromResponseAsync(string url)
        {
            _errorHandler.WriteStringToFuncion("HttpService", nameof(GetDataSetFromResponseAsync));
            AddBearerToken();

            try
            {
                var response = await GetAsync<CResponseMessage1>(url);
                if (response?.Data != null)
                {
                    var json = JsonConvert.SerializeObject(response.Data);
                    return JsonConvert.DeserializeObject<DataSet>(json);
                }
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
            }

            return new DataSet();
        }

        public async Task<CResponseMessage1> PutResponseAsync(string url, object data)
        {
            _errorHandler.WriteStringToFuncion("HttpService", nameof(PutResponseAsync));
            _errorHandler.WriteStringToFile("Put_URL", url);
            _errorHandler.WriteStringToFile("Put_Data", data);

            Console.WriteLine($"[DEBUG] Calling PUT to URL: {url}");
            Console.WriteLine($"[DEBUG] Data object: {JsonConvert.SerializeObject(data)}");

            AddBearerToken();

            try
            {
                var jsonContent = CreateJsonContent(data ?? new { });

                string jsonBody = await jsonContent.ReadAsStringAsync();
                _errorHandler.WriteStringToFile("Put_JsonBody", jsonBody);
                

                var response = await _client.PutAsync(url, jsonContent);

                string json = await response.Content.ReadAsStringAsync();

                _errorHandler.WriteStringToFile("Put_ResponseCode", ((int)response.StatusCode).ToString());
                _errorHandler.WriteStringToFile("Put_ResponseBody", json);

                

                if (!response.IsSuccessStatusCode)
                {
                  
                    return new CResponseMessage1
                    {
                        Success = false,
                        code = ((int)response.StatusCode).ToString(),
                        message = "Cập nhật thất bại hoặc không có quyền."
                    };
                }

                var wrapper = JsonConvert.DeserializeObject<ApiResponseWrapper<CResponseMessage1>>(json);
                var result = wrapper?.result ?? new CResponseMessage1
                {
                    Success = false,
                    code = "500",
                    message = "Không đọc được dữ liệu kết quả từ API"
                };

           
                return result;


            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
               

                return new CResponseMessage1
                {
                    Success = false,
                    code = "500",
                    message = "Lỗi gọi PUT API: " + ex.Message
                };
            }
        }


        public async Task<List<T>> GetTableFromCResponseAsync<T>(string url)
        {
            _errorHandler.WriteStringToFuncion("HttpService", nameof(GetTableFromCResponseAsync));
            AddBearerToken();

            try
            {
                var response = await GetAsync<CResponseMessage1>(url);

                if (response?.Data == null)
                    return new List<T>();

                // Convert object Data => JSON string => deserialize lại thành table object
                var json = JsonConvert.SerializeObject(response.Data);
                var wrapper = JsonConvert.DeserializeObject<TableWrapper<T>>(json);

                return wrapper?.Table ?? new List<T>();
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new List<T>();
            }
        }

        public async Task<CResponseMessage1> DeleteWithBodyResponseAsync(string url, object data)
        {
            _errorHandler.WriteStringToFuncion("HttpService", nameof(DeleteWithBodyResponseAsync));
            AddBearerToken();

            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(_client.BaseAddress + url),
                    Content = CreateJsonContent(data)
                };

                var response = await _client.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                _errorHandler.WriteStringToFile("Response Status", response.StatusCode.ToString());
                _errorHandler.WriteStringToFile("Response Content", json);

           
                var rootObj = JsonConvert.DeserializeObject<dynamic>(json);
                var resultToken = rootObj?.result;

                if (resultToken != null)
                {
                    var resultStr = JsonConvert.SerializeObject(resultToken);
                    return JsonConvert.DeserializeObject<CResponseMessage1>(resultStr);
                }

               
                return JsonConvert.DeserializeObject<CResponseMessage1>(json);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1
                {
                    Success = false,
                    code = "500",
                    message = "Lỗi gọi DELETE API: " + ex.Message
                };
            }
        }

        public async Task<CResponseMessage1> DeleteResponseAsync(string url)
        {
            _errorHandler.WriteStringToFuncion("HttpService", nameof(DeleteResponseAsync));
            AddBearerToken();

            try
            {
                Console.WriteLine($"[DEBUG] Gửi yêu cầu DELETE đến: {url}");

                var response = await _client.DeleteAsync(url);
                Console.WriteLine($"[DEBUG] Status code trả về: {(int)response.StatusCode}");

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[DEBUG] Nội dung JSON trả về:\n{json}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("[DEBUG] Phản hồi không thành công từ server.");
                    return new CResponseMessage1
                    {
                        Success = false,
                        code = ((int)response.StatusCode).ToString(),
                        message = "Xóa thất bại hoặc không có quyền."
                    };
                }

                var wrapper = JsonConvert.DeserializeObject<ApiResponseWrapper<CResponseMessage1>>(json);
                var result = wrapper?.result;

                if (result == null)
                {
                    Console.WriteLine("[DEBUG] Không thể parse dữ liệu từ JSON (result == null)");
                    return new CResponseMessage1
                    {
                        Success = false,
                        code = "500",
                        message = "Không đọc được kết quả từ API."
                    };
                }

                Console.WriteLine($"[DEBUG] Kết quả trả về từ API: Success={result.Success}, Code={result.code}, Message={result.message}");
                return result;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                Console.WriteLine($"[DEBUG] Exception xảy ra khi gọi DELETE API: {ex.Message}");

                return new CResponseMessage1
                {
                    Success = false,
                    code = "500",
                    message = "Lỗi gọi DELETE API: " + ex.Message
                };
            }
        }

        public class ApiResponseWrapper<T>
        {
            public T result { get; set; }
        }


    }
}
