using Newtonsoft.Json;

namespace Restful.Booker.Api.Tests.Models;

public class BookingDatesDto
{
    [JsonProperty("checkin")]
    public string CheckIn { get; set; } = string.Empty;

    [JsonProperty("checkout")]
    public string CheckOut { get; set; } = string.Empty;
}
