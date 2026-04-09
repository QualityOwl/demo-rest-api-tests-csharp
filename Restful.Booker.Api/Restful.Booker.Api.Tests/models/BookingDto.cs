using Newtonsoft.Json;

namespace Restful.Booker.Api.Tests.Models;

public class BookingDto
{
    [JsonProperty("firstname")]
    public string FirstName { get; set; } = string.Empty;

    [JsonProperty("lastname")]
    public string LastName { get; set; } = string.Empty;

    [JsonProperty("totalprice")]
    public int TotalPrice { get; set; }

    [JsonProperty("depositpaid")]
    public bool DepositPaid { get; set; }

    [JsonProperty("bookingdates")]
    public BookingDatesDto BookingDates { get; set; } = new();

    [JsonProperty("additionalneeds")]
    public string? AdditionalNeeds { get; set; }
}
