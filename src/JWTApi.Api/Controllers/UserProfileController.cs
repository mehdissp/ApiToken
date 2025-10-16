using JWTApi.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JWTApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfileController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public UserProfileController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public class UploadPhotoDto
        {
            public IFormFile File { get; set; }
        }
        [HttpPost("upload-photo")]
[Consumes("multipart/form-data")]
public async Task<IActionResult> UploadPhoto([FromForm] UploadPhotoDto file)
{
    if (file == null || file.File.Length == 0)
        return BadRequest(new { message = "هیچ فایلی ارسال نشده است." });

    //var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
    //if (!Directory.Exists(uploadsPath))
    //    Directory.CreateDirectory(uploadsPath);

    //var fileName = $"{Guid.NewGuid()}_{file.FileName}";
    //var filePath = Path.Combine(uploadsPath, fileName);

    //using (var stream = new FileStream(filePath, FileMode.Create))
    //{
    //    await file.CopyToAsync(stream);
    //}

    return Ok(new
    {
        message = "عکس با موفقیت آپلود شد.",
        path = $"/uploads/{file.File.Name}"
    });
}

    }
}
