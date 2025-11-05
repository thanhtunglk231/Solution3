using CommonLib.Handles;
using DataServiceLib.Interfaces1;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Upload;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace DataServiceLib.Implementations1
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private readonly DriveService _driveService;
        private readonly string _folderId; // optional: folder in Shared Drive

        public GoogleDriveService(string serviceAccountJsonPath, string folderId = null)
        {
            if (string.IsNullOrWhiteSpace(serviceAccountJsonPath))
                throw new ArgumentNullException(nameof(serviceAccountJsonPath));

            if (!System.IO.File.Exists(serviceAccountJsonPath))
                throw new FileNotFoundException("Service account json not found.", serviceAccountJsonPath);

            _folderId = folderId; // có thể null => upload vào drive của service account (không khuyến nghị nếu no quota)

            var credential = GoogleCredential.FromFile(serviceAccountJsonPath)
                .CreateScoped(new[] { DriveService.Scope.Drive, DriveService.Scope.DriveFile });

            _driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "MyApp"
            });
        }

        public async Task<string> UploadFileAsync(Stream inputStream, string fileName, string contentType)
        {
            if (_driveService == null) throw new InvalidOperationException("_driveService is null.");

            var meta = new Google.Apis.Drive.v3.Data.File { Name = fileName };
            if (!string.IsNullOrEmpty(_folderId)) meta.Parents = new List<string> { _folderId };

            var request = _driveService.Files.Create(meta, inputStream, contentType);
            request.Fields = "id";
            request.SupportsAllDrives = true;         // cần khi dùng Shared Drive
            //request.IncludeItemsFromAllDrives = true; // hữu ích

            var result = await request.UploadAsync();
            if (result.Status != UploadStatus.Completed)
                throw result.Exception ?? new Exception("Upload failed");

            var file = request.ResponseBody;
            if (file == null || string.IsNullOrEmpty(file.Id))
                throw new Exception("No file id returned from Drive.");

            // set public (tùy nhu cầu)
            var perm = new Permission { Type = "anyone", Role = "reader" };
            var permReq = _driveService.Permissions.Create(perm, file.Id);
            permReq.SupportsAllDrives = true;
            await permReq.ExecuteAsync();

            return $"https://drive.google.com/uc?id={file.Id}";
        }

        //public async Task DeleteFileAsync(string fileId)
        //{
        //    await _service.Files.Delete(fileId).ExecuteAsync();
        //}

        //public async Task<List<Google.Apis.Drive.v3.Data.File>> ListFilesAsync()
        //{
        //    var fileList = _service.Files.List();
        //    fileList.Q = $"mimeType!='application/vnd.google-apps.folder' and '{_folderId}' in parents";
        //    fileList.Fields = "files(id,name,size,mimeType,createdTime)";

        //    var result = await fileList.ExecuteAsync();
        //    return result.Files as List<Google.Apis.Drive.v3.Data.File> ?? new List<Google.Apis.Drive.v3.Data.File>();
        //}
    }
}
