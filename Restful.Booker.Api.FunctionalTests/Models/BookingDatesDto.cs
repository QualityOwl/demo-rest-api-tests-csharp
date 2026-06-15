using System.Text.Json.Serialization;

namespace Restful.Booker.Api.Tests.Models;

public class BookingDatesDto
{
    [JsonPropertyName("checkin")]
    public string CheckIn { get; set; } = string.Empty;

    [JsonPropertyName("checkout")]
    public string CheckOut { get; set; } = string.Empty;
}