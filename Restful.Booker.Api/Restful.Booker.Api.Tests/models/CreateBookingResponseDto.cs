using Newtonsoft.Json;

namespace Restful.Booker.Api.Tests.Models;

public class CreateBookingResponseDto
{
    [JsonProperty("bookingid")]
    public int BookingId { get; set; }

    [JsonProperty("booking")]
    public BookingDto Booking { get; set; } = new();
}
