namespace Infrastructure.Entity.Options;

public class GeminiOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-4o";
    public string BaseUrl { get; set; } = "https://api.openai.com";
    public int TimeoutSeconds { get; set; } = 30;
    public int MaxTokens { get; set; } = 200;
}
