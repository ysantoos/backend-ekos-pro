using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Domain.Service.Interfaces;
using Microsoft.Extensions.Options;

namespace Infrastructure.Entity.Services;

public class GeminiTextGenerationService : ITextGenerationService
{
    private readonly HttpClient _http;
    private readonly Infrastructure.Entity.Options.GeminiOptions _opts;
    private readonly string _promptTemplate;

    public GeminiTextGenerationService(HttpClient http, IOptions<Infrastructure.Entity.Options.GeminiOptions> opts)
    {
        _http = http;
        _opts = opts.Value;
        _http.Timeout = TimeSpan.FromSeconds(_opts.TimeoutSeconds);
        if (!string.IsNullOrWhiteSpace(_opts.ApiKey))
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _opts.ApiKey);

        // Load embedded prompt template file path within the project
        var file = Path.Combine(AppContext.BaseDirectory, "Infrastructure.Entity/Prompts/book_synopsis.txt");
        if (File.Exists(file))
            _promptTemplate = File.ReadAllText(file);
        else
            _promptTemplate = "Create a short English synopsis for the book titled: \"{{Title}}\" by {{Author}}.";
    }

    public async Task<string> GenerateSynopsisAsync(string title, string author, CancellationToken cancellationToken = default)
    {
        title = SanitizeInput(title);
        author = SanitizeInput(author);

        var prompt = _promptTemplate.Replace("{{Title}}", title).Replace("{{Author}}", author);

        var payload = new
        {
            model = _opts.Model,
            prompt = prompt,
            max_tokens = _opts.MaxTokens
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var resp = await _http.PostAsync(new Uri(new Uri(_opts.BaseUrl), "/v1/completions"), content, cancellationToken);
        resp.EnsureSuccessStatusCode();

        using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        if (doc.RootElement.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
        {
            var first = choices[0];
            if (first.TryGetProperty("text", out var text))
                return text.GetString() ?? string.Empty;
        }

        return string.Empty;
    }

    private static string SanitizeInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;
        var cleaned = new string(input.Where(c => !char.IsControl(c)).ToArray());
        if (cleaned.Length > 200) cleaned = cleaned.Substring(0, 200);
        return cleaned;
    }
}
