using System.Text.Json.Serialization;

namespace Restful.Booker.Api.Tests.Models;

public class BookingDto
{
    [JsonPropertyName("firstname")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("lastname")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("totalprice")]
    public int TotalPrice { get; set; }

    [JsonPropertyName("depositpaid")]
    public bool DepositPaid { get; set; }

    [JsonPropertyName("bookingdates")]
    public BookingDatesDto BookingDates { get; set; } = new();

    [JsonPropertyName("additionalneeds")]
    public string? AdditionalNeeds { get; set; }
}