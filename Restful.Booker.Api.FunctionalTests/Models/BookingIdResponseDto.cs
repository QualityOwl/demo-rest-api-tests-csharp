using System.Text.Json.Serialization;

namespace Restful.Booker.Api.Tests.Models;

public class BookingIdResponseDto
{
    [JsonPropertyName("bookingid")]
    public int BookingId { get; set; }
}