using DataServiceLib.Hubs;
using DataServiceLib.Implementations1;
using DataServiceLib.Interfaces1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IGoogleDriveService _driveService;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ISupabaseService _supabaseService;
        public FilesController(IGoogleDriveService driveService, IHubContext<ChatHub> hubContext, ISupabaseService supabaseService)
        {
            _driveService = driveService;
            _hubContext = hubContext;
            _supabaseService = supabaseService;
        }

        [HttpPost("uploadSupaBase")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File không hợp lệ");

            var url = await _supabaseService.UploadFileAsync(file);

            return Ok(new { Url = url });
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Uploadggdr(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest();

            using var stream = file.OpenReadStream();
            var fileUrl = await _driveService.UploadFileAsync(stream, file.FileName, file.ContentType);

            var type = file.ContentType.StartsWith("image") ? "image" :
                       file.ContentType.StartsWith("video") ? "video" :
                       file.ContentType.StartsWith("audio") ? "audio" : "file";

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "system", fileUrl, type);

            return Ok(new { url = fileUrl });
        }
    }
}
