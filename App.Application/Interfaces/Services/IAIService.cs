using App.Application.Dto;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Interfaces.Services
{

    public interface IAIService
    {
        Task<dynamic> GenerateNutrientDetails(string filePath);
        Task<JObject> GetAIResponseAsync(FoodData prompt);
    }
}
