using App.Application.Dto;

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