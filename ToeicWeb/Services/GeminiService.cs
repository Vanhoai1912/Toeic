using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;

namespace ToeicWeb.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["GoogleGemini:ApiKey"];
        }

        public async Task<string> GetChatbotResponse(string prompt)
        {
            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                }
            };

            var jsonRequest = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var url = $"https://generativelanguage.googleapis.com/v1/models/gemini-pro:generateContent?key={_apiKey}";
            var response = await _httpClient.PostAsync(url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine("🔹 API Request: " + jsonRequest);
            Console.WriteLine("🔹 API Response: " + responseBody);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    return jsonResponse
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("❌ Lỗi parse JSON: " + ex.Message);
                    Console.WriteLine("⚠ API Response bị sai format: " + responseBody);
                }
            }
            else
            {
                Console.WriteLine("❌ Lỗi API: " + response.StatusCode);
            }

            return "Lỗi khi gọi API Gemini!";
        }
    }
}
