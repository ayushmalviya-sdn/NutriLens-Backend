using App.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

        public class AIResponse
        {
            public string FoodName { get; set; }
            public string FoodCategory { get; set; }
            public int CaloriesPerServing { get; set; }
            public string ServingSize { get; set; }
            public Macros Macros { get; set; }
            public int HealthinessScore { get; set; }
            public List<string> HealthierAlternatives { get; set; }
            public int ConfidenceScore { get; set; }
        }

        public class Macros
        {
            public double Protein { get; set; }
            public double Carbs { get; set; }
            public double Fat { get; set; }
            public double Fiber { get; set; }
            public double Sugar { get; set; }
        }

        [HttpPost("upload")]
        public async Task<ActionResult<AIResponse>> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                var fileUrl = await _fileUploadService.UploadFileAsync(file);
                var aiResponse = await _aiService.GenerateNutrientDetails(fileUrl);

                var result = JsonConvert.SerializeObject(aiResponse);
                Console.WriteLine(result);  // Or use ILogger to log
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("generate-response")]
        public async Task<IActionResult> GenerateResponse([FromBody] string prompt)
        {
            var response = await _aiService.GetAIResponseAsync(prompt);
            return Ok(new { Response = response });
        }
    }
}
