using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ToeicWeb.Services
{
    public class ChatbotService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public ChatbotService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiUrl = configuration["Chatbot:ApiUrl"]; // Lấy URL API từ appsettings.json

            if (string.IsNullOrEmpty(_apiUrl))
            {
                throw new ArgumentException("API URL cho chatbot không được để trống.");
            }
        }

        public async Task<string> GetChatbotResponse(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                return "Nội dung câu hỏi không hợp lệ.";
            }

            var requestBody = new { question = prompt };
            var jsonRequest = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");


            try
            {
                var response = await _httpClient.PostAsync(_apiUrl, content);
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response from API: " + responseBody);



                if (response.IsSuccessStatusCode)
                {
                    return responseBody;
                }
                else
                {
                    Console.WriteLine($"Lỗi API Flask: {response.StatusCode}, Nội dung: {responseBody}");
                    return $"Lỗi khi gọi API chatbot: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gọi API chatbot: {ex.Message}");
                return "Đã xảy ra lỗi khi kết nối tới API.";
            }
        }
    }
}
