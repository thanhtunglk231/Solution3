using CoreLib.Dtos;
using DataServiceLib.Implementations1;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Supabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServiceLib.Interfaces1
{
    public class SupabaseService : ISupabaseService
    {
        private readonly RestClient _client;

        public SupabaseService()
        {
            _client = new RestClient(SupabaseConfig.SupabaseUrl);
        }

        public async Task<string?> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            var fileName = $"products/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var uploadUrl = $"{SupabaseConfig.SupabaseUrl}/storage/v1/object/{SupabaseConfig.StorageBucket}/{fileName}";

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var request = new RestRequest(uploadUrl, RestSharp.Method.Put);
            request.AddHeader("apikey", SupabaseConfig.SupabaseApiKey);
            request.AddHeader("Authorization", $"Bearer {SupabaseConfig.SupabaseApiKey}");
            request.AddHeader("Content-Type", file.ContentType);

            request.AddParameter(file.ContentType, memoryStream.ToArray(), ParameterType.RequestBody);

            var response = await _client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var publicUrl = $"{SupabaseConfig.SupabaseUrl}/storage/v1/object/public/{SupabaseConfig.StorageBucket}/{fileName}";
                return publicUrl;
            }
            else
            {
                Console.WriteLine("Supabase Upload Error: " + response.Content);
                return null;
            }
        }

      
        /// Xoá 1 object trong bucket. Tham số có thể là public URL hoặc đường dẫn tương đối trong bucket (ví dụ: "products/abc.png").
        /// Trả về true nếu xoá thành công.
        
        public async Task<bool> DeleteFileAsync(string pathOrUrl)
        {
            if (string.IsNullOrWhiteSpace(pathOrUrl))
                return false;

            // Chuẩn hoá về object path bên trong bucket
            string objectPath = NormalizeObjectPath(pathOrUrl);

            // Encode từng segment để tránh lỗi khi có khoảng trắng, ký tự UTF-8...
            string encodedPath = EncodeObjectPath(objectPath);

            var deleteUrl = $"{SupabaseConfig.SupabaseUrl}/storage/v1/object/{SupabaseConfig.StorageBucket}/{encodedPath}";

            var request = new RestRequest(deleteUrl, Method.Delete);
            request.AddHeader("apikey", SupabaseConfig.SupabaseApiKey);
            request.AddHeader("Authorization", $"Bearer {SupabaseConfig.SupabaseApiKey}");

            var response = await _client.ExecuteAsync(request);

            if (response.IsSuccessful)
                return true;

            Console.WriteLine($"Supabase Delete Error: {(int)response.StatusCode} - {response.Content}");
            return false;
        }

        private static string NormalizeObjectPath(string pathOrUrl)
        {
            // Các prefix có thể gặp
            // public:  {url}/storage/v1/object/public/{bucket}/{object}
            // private: {url}/storage/v1/object/{bucket}/{object}
            string trimmed = pathOrUrl.Trim();

            // Nếu người dùng truyền public URL
            // => cắt prefix để giữ lại phần {object}
            int idxPublic = trimmed.IndexOf("/storage/v1/object/public/", StringComparison.OrdinalIgnoreCase);
            if (idxPublic >= 0)
            {
                // sau đoạn "/public/" là "{bucket}/{object}"
                var afterPublic = trimmed.Substring(idxPublic + "/storage/v1/object/public/".Length);
                // bỏ "{bucket}/"
                var idxSlash = afterPublic.IndexOf('/');
                if (idxSlash >= 0) return afterPublic.Substring(idxSlash + 1);
                return string.Empty;
            }

            // Nếu người dùng truyền private URL
            int idxPrivate = trimmed.IndexOf("/storage/v1/object/", StringComparison.OrdinalIgnoreCase);
            if (idxPrivate >= 0)
            {
                var after = trimmed.Substring(idxPrivate + "/storage/v1/object/".Length); // "{bucket}/{object}"
                var idxSlash = after.IndexOf('/');
                if (idxSlash >= 0) return after.Substring(idxSlash + 1);
                return string.Empty;
            }

            // Ngược lại coi như họ truyền thẳng object path
            return trimmed.TrimStart('/');
        }

        private static string EncodeObjectPath(string objectPath)
        {
            // Encode từng segment, giữ nguyên dấu '/'
            var segments = objectPath
                .Split('/', StringSplitOptions.RemoveEmptyEntries)
                .Select(Uri.EscapeDataString);
            return string.Join("/", segments);
        }
    }
}
