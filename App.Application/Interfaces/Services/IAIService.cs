using Microsoft.AspNetCore.Http;
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
    }
}
