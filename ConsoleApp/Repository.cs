using System.Text.Json.Serialization;

namespace ConsoleApp;

public record class Repository(
        [property: JsonPropertyName("id")] string id);