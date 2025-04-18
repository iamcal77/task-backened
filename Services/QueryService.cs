using MyWebAPI.Models;
using System.Text.Json;
using System.Text;

namespace MyWebAPI.Services
{
    public class QueryService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public QueryService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["OpenAI:ApiKey"]; // Retrieve API key from configuration
        }

        public async Task<string> GetLLMResponseAsync(string question)
        {
            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[] {
                    new { role = "user", content = question }
                },
                max_tokens = 500,
                temperature = 0.7
            };

            var requestContent = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", requestContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}");
                Console.WriteLine($"Details: {errorJson}");
                return "Sorry, something went wrong while fetching the response from ChatGPT.";
            }

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            var content = doc.RootElement
                             .GetProperty("choices")[0]
                             .GetProperty("message")
                             .GetProperty("content")
                             .GetString();

            return content?.Trim() ?? "No answer found.";
        }
    }
}
