using System.Text.Json.Serialization;


/// <summary>
/// API yanıtındaki mesaj içeriği modeli.
/// </summary>
public partial class Message
        {
            [JsonPropertyName("content")]
            public required string Content { get; set; }
        }