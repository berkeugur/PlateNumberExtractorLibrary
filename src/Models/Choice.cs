using System.Text.Json.Serialization;


/// <summary>
/// API yanıtı içerisindeki seçim modeli.
/// </summary>
public partial class Choice
        {
            [JsonPropertyName("message")]
            public required Message Message { get; set; }
        }