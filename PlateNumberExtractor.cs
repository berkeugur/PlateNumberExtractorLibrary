using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PlateNumberExtractorLibrary
{
    public class PlateNumberExtractor
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey;

        public PlateNumberExtractor(IHttpClientFactory httpClientFactory, string apiKey)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public async Task<string> ExtractPlateNumberFromBase64Image(string base64Image)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("OpenAI API key is not configured.");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = new object[]
                        {
                            new { type = "text", text = "Bu görselde plaka numarası nedir? Sadece plaka numarasını yazınız. Örneğin: 34ABC123, başka bir şey yazmayın." },
                            new { type = "image_url", image_url = new { url = $"data:image/jpeg;base64,{base64Image}" } }
                        }
                    }
                }
            };

            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var completion = JsonSerializer.Deserialize<OpenAIResponse>(responseBody);

            if (completion?.Choices == null || completion.Choices.Length == 0 || completion.Choices[0].Message == null)
            {
                throw new InvalidOperationException("Invalid response from OpenAI API.");
            }

            return completion.Choices[0].Message.Content.Trim();
        }

        public partial class OpenAIResponse
        {
            [JsonPropertyName("choices")]
            public required Choice[] Choices { get; set; }
        }

        public partial class Choice
        {

            [JsonPropertyName("message")]
            public required Message Message { get; set; }
        }

        public partial class Message
        {
            [JsonPropertyName("content")]
            public required string Content { get; set; }

        }
    }
}