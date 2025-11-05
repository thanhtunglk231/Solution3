
using Microsoft.AspNetCore.Http;

namespace DataServiceLib.Implementations1
{
    public interface ISupabaseService
    {
        Task<string?> UploadFileAsync(IFormFile file);
    }
}