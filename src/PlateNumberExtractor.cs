using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PlateNumberExtractorLibrary
{
    /// <summary>
    /// PlateNumberExtractor sınıfı, görüntülerden plaka numarası çıkarmak için OpenAI API'sini kullanan bir yardımcı kütüphanedir.
    /// </summary>
    public class PlateNumberExtractor
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey;

        /// <summary>
        /// PlateNumberExtractor sınıfının yapıcı metodu.
        /// </summary>
        /// <param name="httpClientFactory">HTTP istemcisi fabrikası.</param>
        /// <param name="apiKey">OpenAI API anahtarı.</param>
        /// <exception cref="ArgumentNullException">httpClientFactory veya apiKey null olduğunda atılır.</exception>
        public PlateNumberExtractor(IHttpClientFactory httpClientFactory, string apiKey)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        /// <summary>
        /// Base64 kodlu bir görüntüden plaka numarasını çıkarır.
        /// </summary>
        /// <param name="base64Image">Base64 kodlu görüntü.</param>
        /// <returns>Plaka numarası.</returns>
        /// <exception cref="InvalidOperationException">API anahtarı yapılandırılmamışsa veya API yanıtı geçersizse atılır.</exception>
        public async Task<string> ExtractPlateNumberFromBase64Image(string base64Image)
        {
            // API anahtarı kontrolü
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("OpenAI API key is not configured.");
            }

            // HTTP istemcisi oluştur
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            // API'ye gönderilecek veri yapısı
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

            // API'ye POST isteği gönder
            var response = await client.PostAsync(
                "https://api.openai.com/v1/chat/completions",
                new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json")
            );

            // Başarısız bir durumda hata fırlat
            response.EnsureSuccessStatusCode();

            // Yanıtı al ve ayrıştır
            var responseBody = await response.Content.ReadAsStringAsync();
            var completion = JsonSerializer.Deserialize<OpenAIResponse>(responseBody);

            // API yanıtını kontrol et
            if (completion?.Choices == null || completion.Choices.Length == 0 || completion.Choices[0].Message == null)
            {
                throw new InvalidOperationException("Invalid response from OpenAI API.");
            }

            // Çıkarılan plaka numarasını döndür
            return completion.Choices[0].Message.Content.Trim();
        }

        

       

      
    }
}
