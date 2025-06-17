using CoreLib.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

public class DepartmentService : IDepartmentService
{
    private readonly string _baseUrl;
    private readonly HttpClient _client;

    public DepartmentService(IConfiguration configuration, HttpClient client)
    {
        _baseUrl = configuration["PathStrings:Url"];
        _client = client;
    }

    public async Task<List<Department>> GetDeptbyid(int id, string token)
    {
        var url = _baseUrl + "department/" + id;

        // ✅ Gắn JWT token
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Department>>(json);
        }

        return new List<Department>();
    }


    public async Task<List<Department>> GetAll(string token)
    {
        var url = _baseUrl + "department/getall";
        Console.WriteLine(url);

        // ✅ Gắn JWT token
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var setting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
            };

            return JsonConvert.DeserializeObject<List<Department>>(json, setting);
        }

        return new List<Department>();
    }

}
