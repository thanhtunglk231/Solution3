namespace DataServiceLib.Interfaces1
{
    public interface IGoogleDriveService
    {
        Task<string> UploadFileAsync(Stream inputStream, string fileName, string contentType);
    }
}