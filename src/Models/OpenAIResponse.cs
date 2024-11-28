using System.Text.Json.Serialization;


/// <summary>
/// OpenAI API yanıt modeli.
/// </summary>
public partial class OpenAIResponse
        {
            [JsonPropertyName("choices")]
            public required Choice[] Choices { get; set; }
        }