using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WebBrowser.Services.Interfaces;

namespace WebBrowser.Services.Implements
{
    public class FileService : IFileService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _apiBase;          // ví dụ: https://localhost:7182
        private readonly string _uploadEndpoint;   // ví dụ: /api/files hoặc /uploadSupaBase

        public FileService(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _apiBase = config["PathStrings:Url"]?.TrimEnd('/') ?? "";               // cùng nguồn với HttpService
            _uploadEndpoint = config["ApiEndpoints:UploadFile"] ?? "Files/uploadSupaBase";    // tuỳ bạn cấu hình
        }

        private sealed class SupabaseUrlResponse { public string? url { get; set; } }
        public async Task<string?> GetUrlSupabaseAsync(IFormFile file, CancellationToken ct = default)
        {
            if (file == null || file.Length == 0) return null;

            var client = _httpClientFactory.CreateClient();

            var token = _httpContextAccessor.HttpContext?.Session?.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var form = new MultipartFormDataContent();
            using var stream = file.OpenReadStream();
            using var sc = new StreamContent(stream);
            sc.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
            form.Add(sc, "file", file.FileName);

            // Gọi trực tiếp URL backend
            var uploadUrl = "https://localhost:7053/api/Files/uploadSupaBase";
            var res = await client.PostAsync(uploadUrl, form, ct);
            var json = await res.Content.ReadAsStringAsync(ct);

            if (!res.IsSuccessStatusCode)
            {
                Console.WriteLine($"Upload FAIL {res.StatusCode}: {json}");
                return null;
            }

            // Backend trả { "url": "..." }
            var dto = JsonConvert.DeserializeObject<SupabaseUrlResponse>(json);
            return dto?.url;
        }

    }
}
