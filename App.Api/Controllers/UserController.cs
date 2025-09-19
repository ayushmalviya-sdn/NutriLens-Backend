using App.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class MainController : BaseController
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IAIService _aiService;

        // Correct constructor syntax
        public MainController(IFileUploadService fileUploadService, IAIService aiService)
        {
            _fileUploadService = fileUploadService;
            _aiService = aiService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                var fileUrl = await _fileUploadService.UploadFileAsync(file);
                var aiResponse = await _aiService.GenerateNutrientDetails(fileUrl);
                return Ok(new { message = "File uploaded successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
