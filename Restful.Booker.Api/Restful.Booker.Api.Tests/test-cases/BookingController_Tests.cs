using FluentAssertions;
using Newtonsoft.Json;
using Restful.Booker.Api.Tests.Fixtures;
using Restful.Booker.Api.Tests.Models;
using System.Net;
using System.Text;

namespace Restful.Booker.Api.Tests;

public class BookingController_Tests : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;

    public BookingController_Tests(TestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GET_booking_ValidRequest_ShouldReturnBookingList()
    {
        // Act
        var response = await _fixture.HttpClient.GetAsync("/booking");
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseBody.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GET_booking_WithValidId_ShouldReturnCorrectBooking()
    {
        // Arrange - Create a booking first
        var booking = new BookingDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            TotalPrice = 200,
            DepositPaid = false,
            BookingDates = new BookingDatesDto
            {
                CheckIn = "2024-02-01",
                CheckOut = "2024-02-03"
            }
        };

        var createContent = new StringContent(
            JsonConvert.SerializeObject(booking),
            Encoding.UTF8,
            "application/json");

        var createResponse = await _fixture.HttpClient.PostAsync("/booking", createContent);
        var createBody = await createResponse.Content.ReadAsStringAsync();
        var created = JsonConvert.DeserializeObject<CreateBookingResponseDto>(createBody);

        // Act
        var response = await _fixture.HttpClient.GetAsync($"/booking/{created!.BookingId}");
        var responseBody = await response.Content.ReadAsStringAsync();
        var retrievedBooking = JsonConvert.DeserializeObject<BookingDto>(responseBody);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        retrievedBooking.Should().NotBeNull();
        retrievedBooking!.FirstName.Should().Be(booking.FirstName);
        retrievedBooking.LastName.Should().Be(booking.LastName);
    }

    [Fact]
    public async Task GET_booking_WithInvalidId_ShouldReturn404NotFound()
    {
        // Arrange
        var bookingId = 9999999999;

        // Act
        var response = await _fixture.HttpClient.GetAsync($"/booking/{bookingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task POST_booking_WithValidData_ShouldCreateBooking()
    {
        // Arrange
        var booking = new BookingDto
        {
            FirstName = "John",
            LastName = "Doe",
            TotalPrice = 150,
            DepositPaid = true,
            BookingDates = new BookingDatesDto
            {
                CheckIn = "2024-01-01",
                CheckOut = "2024-01-05"
            },
            AdditionalNeeds = "Breakfast"
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(booking),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _fixture.HttpClient.PostAsync("/booking", content);
        var responseBody = await response.Content.ReadAsStringAsync();
        var createdBooking = JsonConvert.DeserializeObject<CreateBookingResponseDto>(responseBody);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        createdBooking.Should().NotBeNull();
        createdBooking!.BookingId.Should().BeGreaterThan(0);
        createdBooking.Booking.FirstName.Should().Be(booking.FirstName);
        createdBooking.Booking.LastName.Should().Be(booking.LastName);
        createdBooking.Booking.TotalPrice.Should().Be(booking.TotalPrice);
    }

    [Fact]
    public async Task PUT_booking_WithValidData_ShouldUpdateExistingBooking()
    {
        // Arrange - Create a booking first
        var originalBooking = new BookingDto
        {
            FirstName = "Original",
            LastName = "Name",
            TotalPrice = 100,
            DepositPaid = true,
            BookingDates = new BookingDatesDto
            {
                CheckIn = "2024-03-01",
                CheckOut = "2024-03-05"
            }
        };

        var createContent = new StringContent(
            JsonConvert.SerializeObject(originalBooking),
            Encoding.UTF8,
            "application/json");

        var createResponse = await _fixture.HttpClient.PostAsync("/booking", createContent);
        var createBody = await createResponse.Content.ReadAsStringAsync();
        var created = JsonConvert.DeserializeObject<CreateBookingResponseDto>(createBody);

        // Get auth token
        var token = await _fixture.GetAuthTokenAsync();

        // Update the booking
        var updatedBooking = new BookingDto
        {
            FirstName = "Updated",
            LastName = "Name",
            TotalPrice = 250,
            DepositPaid = false,
            BookingDates = new BookingDatesDto
            {
                CheckIn = "2024-03-01",
                CheckOut = "2024-03-10"
            }
        };

        var updateContent = new StringContent(
            JsonConvert.SerializeObject(updatedBooking),
            Encoding.UTF8,
            "application/json");

        var request = new HttpRequestMessage(HttpMethod.Put, $"/booking/{created!.BookingId}")
        {
            Content = updateContent
        };
        request.Headers.Add("Cookie", $"token={token}");

        // Act
        var updateResponse = await _fixture.HttpClient.SendAsync(request);
        var updateBody = await updateResponse.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BookingDto>(updateBody);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.FirstName.Should().Be(updatedBooking.FirstName);
        result.TotalPrice.Should().Be(updatedBooking.TotalPrice);
    }

    [Fact]
    public async Task POST_booking_WithValidId_ShouldDeleteCorrectBooking()
    {
        // Arrange - Create a booking first
        var booking = new BookingDto
        {
            FirstName = "ToDelete",
            LastName = "User",
            TotalPrice = 75,
            DepositPaid = true,
            BookingDates = new BookingDatesDto
            {
                CheckIn = "2024-04-01",
                CheckOut = "2024-04-02"
            }
        };

        var createContent = new StringContent(
            JsonConvert.SerializeObject(booking),
            Encoding.UTF8,
            "application/json");

        var createResponse = await _fixture.HttpClient.PostAsync("/booking", createContent);
        var createBody = await createResponse.Content.ReadAsStringAsync();
        var created = JsonConvert.DeserializeObject<CreateBookingResponseDto>(createBody);

        // Get auth token
        var token = await _fixture.GetAuthTokenAsync();

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/booking/{created!.BookingId}");
        request.Headers.Add("Cookie", $"token={token}");

        // Act
        var deleteResponse = await _fixture.HttpClient.SendAsync(request);

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.Created); // API returns 201 for successful delete
    }
}