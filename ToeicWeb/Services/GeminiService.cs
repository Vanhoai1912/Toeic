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
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new ArgumentException("API key cho Google Gemini không được để trống.");
            }
        }

        public async Task<string> GetChatbotResponse(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                return "Nội dung câu hỏi không hợp lệ.";
            }

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

            try
            {
                var response = await _httpClient.PostAsync(url, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);

                    if (jsonResponse.TryGetProperty("candidates", out var candidates) &&
                        candidates.GetArrayLength() > 0 &&
                        candidates[0].TryGetProperty("content", out var contentProperty) &&
                        contentProperty.TryGetProperty("parts", out var parts) &&
                        parts.GetArrayLength() > 0 &&
                        parts[0].TryGetProperty("text", out var text))
                    {
                        return text.GetString() ?? "AI không trả lời.";
                    }

                    return "Phản hồi từ AI không đúng định dạng.";
                }
                else
                {
                    Console.WriteLine($"Lỗi API: {response.StatusCode}, Nội dung: {responseBody}");
                    return $"Lỗi khi gọi API Gemini: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gọi API Gemini: {ex.Message}");
                return "Đã xảy ra lỗi khi kết nối tới API.";
            }
        }
    }
}
