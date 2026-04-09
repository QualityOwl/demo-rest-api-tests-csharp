using Newtonsoft.Json;

namespace Restful.Booker.Api.Tests.Models;

public class AuthResponseDto
{
    [JsonProperty("token")]
    public string Token { get; set; } = string.Empty;
}
