using System.Text.Json.Serialization;

namespace Restful.Booker.Api.Tests.Models;

public class CreateBookingResponseDto
{
    [JsonPropertyName("bookingid")]
    public int BookingId { get; set; }

    [JsonPropertyName("booking")]
    public BookingDto Booking { get; set; } = new();
}