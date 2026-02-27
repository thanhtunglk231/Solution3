using CommonLib.Handles;
using CommonLib.Helpers;
using Microsoft.AspNetCore.Mvc;
using WebBrowser.Services.Implements;
using WebBrowser.Services.Interfaces;

namespace WebBrowser.Controllers
{
    public class FileController : BaseController
    {
        private readonly IFileService _fileService;
        private readonly IErrorHandler _errorHandler;
        private readonly IDataConvertHelper _dataConvertHelper;

        public FileController(IFileService fileService, IConfiguration configuration,
                              IErrorHandler errorHandler, IDataConvertHelper dataConvertHelper)
            : base(configuration)
        {
            _fileService = fileService;
            _errorHandler = errorHandler;
            _dataConvertHelper = dataConvertHelper;
        }

        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Username");
            ViewBag.Username = username;

            var token = GetJwtTokenOrRedirect(out IActionResult? redirectResult);
            if (token == null)
                return redirectResult!;

            return View();
        }
        [IgnoreAntiforgeryToken]                 // nếu bạn bật antiforgery toàn cục
        [Consumes("multipart/form-data")]        // ép formatter nhận form-data (tránh 415)
        // (Tuỳ chọn) tăng giới hạn nếu cần upload lớn:
        // [RequestFormLimits(MultipartBodyLengthLimit = 100 * 1024 * 1024)]
        public async Task<IActionResult> UploadFromBrowser([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File không hợp lệ");

            try
            {
                var url = await _fileService.GetUrlSupabaseAsync(file);
                if (string.IsNullOrEmpty(url))
                    return BadRequest("Upload thất bại");

                return Ok(new { url });
            }
            catch (Exception ex)
            {
                _errorHandler?.WriteToFile(ex);
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }
    }
}