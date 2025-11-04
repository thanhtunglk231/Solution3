namespace WebBrowser.Services.Interfaces
{
    public interface IFileService
    {
        Task<string?> GetUrlSupabaseAsync(IFormFile file, CancellationToken ct = default);
    }
}