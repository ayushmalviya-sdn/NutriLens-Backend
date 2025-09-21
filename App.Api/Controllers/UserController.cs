using App.Application.Dto;
using App.Application.Interfaces.Services;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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



        [HttpPost("[action]")]
        public async Task<ActionResult<AIResponse>> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                var fileUrl = await _fileUploadService.UploadFileAsync(file);
                var aiResponse = await _aiService.GenerateNutrientDetails(fileUrl);

                var result = JsonConvert.SerializeObject(aiResponse);
                Console.WriteLine(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GenerateResponse([FromBody] FoodData foodData)
        {

            JObject response = await _aiService.GetAIResponseAsync(foodData);

            if (response == null)
                return BadRequest(new { error = "Failed to get valid JSON from AI model." });

            // Return the processed response as JSON
            return new ContentResult
            {
                Content = response.ToString(),
                ContentType = "application/json",
                StatusCode = 200
            };
        }
    }
}