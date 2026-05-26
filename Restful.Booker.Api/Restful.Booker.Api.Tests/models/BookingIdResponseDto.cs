using Newtonsoft.Json;

namespace Restful.Booker.Api.Tests.Models;

public class BookingIdResponseDto
{
    [JsonProperty("bookingid")]
    public int BookingId { get; set; }
}