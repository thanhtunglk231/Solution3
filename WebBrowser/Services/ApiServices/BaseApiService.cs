using CommonLib.Interfaces;
using Newtonsoft.Json;
using Serilog;
using System.Text;

namespace WebBrowser.Services.ApiServices
{
    public abstract class BaseApiService
    {
        protected readonly HttpClient _client;
        protected readonly string _baseUrl;
        private readonly IErrorHandler _errorHandler;
        protected BaseApiService(IConfiguration configuration, HttpClient client)
        {
            _client = client;
            _baseUrl = configuration["PathStrings:Url"];
        }

        protected async Task<T?> GetAsync<T>(string relativeUrl)
        {
            string? json = null;

            try
            {
                var fullUrl = _baseUrl + relativeUrl;
                Log.Information("Sending GET request to: {Url}", fullUrl);

                var response = await _client.GetAsync(fullUrl);

                if (!response.IsSuccessStatusCode)
                {
                    Log.Warning("GET request failed with status code {StatusCode} for URL {Url}", response.StatusCode, fullUrl);
                    return default;
                }

                json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<T>(json);

                Log.Information("GET request to {Url} succeeded", fullUrl);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while sending GET request to {Url}. Raw JSON: {Json}", _baseUrl + relativeUrl, json);
                return default;
            }
        }

        protected async Task<T?> PostAsync<T>(string relativeUrl)
        {
            try
            {
                var response = await _client.PostAsync(_baseUrl + relativeUrl, null);

                if (!response.IsSuccessStatusCode)
                {
                    Log.Warning("GET request failed with status code {StatusCode} for URL {Url}", response.StatusCode, _baseUrl + relativeUrl);
                    return default;
                }

                var json = await response.Content.ReadAsStringAsync();
                Log.Information("GET request to {Url} succeeded", _baseUrl + relativeUrl);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                this._errorHandler.WriteToFile(ex);
                return default;
            }

        }
        protected async Task<T?> DeleteAsync<T>(string relativeUrl)
        {
            try
            {
                var response = await _client.DeleteAsync(_baseUrl + relativeUrl);

                if (!response.IsSuccessStatusCode)
                {
                    Log.Warning("GET request failed with status code {StatusCode} for URL {Url}", response.StatusCode, _baseUrl + relativeUrl);
                    return default;
                }

                var json = await response.Content.ReadAsStringAsync();
                Log.Information("GET request to {Url} succeeded", _baseUrl + relativeUrl);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                this._errorHandler.WriteToFile(ex);
                return default;
            }
        }
        protected async Task<T?> DeleteJsonAsync<T>(string relativeUrl, object body)
        {
            try
            {

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(_baseUrl + relativeUrl),
                    Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")
                };

                var response = await _client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return default;

                }


                var json = await response.Content.ReadAsStringAsync();
                Log.Information("GET request to {Url} succeeded", _baseUrl + relativeUrl);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {


                this._errorHandler.WriteToFile(ex);
                return default;
            }
        }




        protected async Task<T?> PutAsync<T>(string relativeUrl)
        {
            try
            {
                var response = await _client.PutAsync(_baseUrl + relativeUrl, null);

                if (!response.IsSuccessStatusCode)
                {
                    Log.Warning("GET request failed with status code {StatusCode} for URL {Url}", response.StatusCode, _baseUrl + relativeUrl);
                    return default;
                }

                var json = await response.Content.ReadAsStringAsync();
                Log.Information("GET request to {Url} succeeded", _baseUrl + relativeUrl);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                this._errorHandler.WriteToFile(ex);
                return default;
            }
        }
        protected async Task<T?> PutJsonAsync<T>(string relativeUrl, object body)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                var response = await _client.PutAsync(_baseUrl + relativeUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    Log.Warning("GET request failed with status code {StatusCode} for URL {Url}", response.StatusCode, _baseUrl + relativeUrl);
                    return default;
                }

                var json = await response.Content.ReadAsStringAsync();
                Log.Information("GET request to {Url} succeeded", _baseUrl + relativeUrl);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                this._errorHandler.WriteToFile(ex);
                return default;
            }
        }


        protected async Task<T?> PostJsonAsync<T>(string relativeUrl, object body)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(_baseUrl + relativeUrl, content);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Log.Warning("POST request failed with status code {StatusCode} for URL {Url}. Response: {Response}",
                        response.StatusCode, _baseUrl + relativeUrl, json);


                    try
                    {
                        return JsonConvert.DeserializeObject<T>(json);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Không thể parse lỗi JSON về kiểu {Type}. Error: {Error}", typeof(T).Name, ex.Message);
                        return default;
                    }
                }

                Log.Information("POST request to {Url} succeeded", _baseUrl + relativeUrl);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                this._errorHandler?.WriteToFile(ex);
                return default;
            }
        }

    }

}