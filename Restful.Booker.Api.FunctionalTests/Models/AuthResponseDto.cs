using System.Text.Json.Serialization;

namespace Restful.Booker.Api.Tests.Models;

public class AuthResponseDto
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
}