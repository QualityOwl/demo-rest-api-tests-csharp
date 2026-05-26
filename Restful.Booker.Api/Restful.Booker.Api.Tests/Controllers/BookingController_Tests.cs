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

        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        created.Should().NotBeNull();
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

        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        created.Should().NotBeNull();

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
    public async Task DELETE_booking_WithValidId_ShouldDeleteCorrectBooking()
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

        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        created.Should().NotBeNull();

        // Get auth token
        var token = await _fixture.GetAuthTokenAsync();

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/booking/{created.BookingId}");
        request.Headers.Add("Cookie", $"token={token}");

        // Act
        var deleteResponse = await _fixture.HttpClient.SendAsync(request);

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.Created); // API returns 201 for successful delete
    }

    [Fact]
    public async Task GET_booking_ValidRequest_ShouldReturnArrayOfBookingIds()
    {
        // Act
        var response = await _fixture.HttpClient.GetAsync("/booking");
        var responseBody = await response.Content.ReadAsStringAsync();
        var bookingIds = JsonConvert.DeserializeObject<List<BookingIdResponseDto>>(responseBody);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        bookingIds.Should().NotBeNull();
        bookingIds!.Should().NotBeEmpty();
        bookingIds.Should().AllSatisfy(b => b.BookingId.Should().BeGreaterThan(0));
    }

    [Fact]
    public async Task GET_booking_FilterByFirstName_ShouldReturnMatchingBookings()
    {
        // Arrange - Create a booking with a unique first name
        var uniqueFirstName = $"FN{Guid.NewGuid():N}"[..14];
        var booking = new BookingDto
        {
            FirstName = uniqueFirstName,
            LastName = "FilterTest",
            TotalPrice = 100,
            DepositPaid = false,
            BookingDates = new BookingDatesDto
            {
                CheckIn = "2099-01-01",
                CheckOut = "2099-01-05"
            }
        };

        var createContent = new StringContent(
            JsonConvert.SerializeObject(booking),
            Encoding.UTF8,
            "application/json");

        var createResponse = await _fixture.HttpClient.PostAsync("/booking", createContent);
        var createBody = await createResponse.Content.ReadAsStringAsync();
        var created = JsonConvert.DeserializeObject<CreateBookingResponseDto>(createBody);

        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        created.Should().NotBeNull();

        // Act
        var response = await _fixture.HttpClient.GetAsync($"/booking?firstname={uniqueFirstName}");
        var responseBody = await response.Content.ReadAsStringAsync();
        var bookingIds = JsonConvert.DeserializeObject<List<BookingIdResponseDto>>(responseBody);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        bookingIds.Should().NotBeNull();
        bookingIds!.Should().Contain(b => b.BookingId == created!.BookingId);
    }

    [Fact]
    public async Task GET_booking_FilterByLastName_ShouldReturnMatchingBookings()
    {
        // Arrange - Create a booking with a unique last name
        var uniqueLastName = $"LN{Guid.NewGuid():N}"[..14];
        var booking = new BookingDto
        {
            FirstName = "FilterTest",
            LastName = uniqueLastName,
            TotalPrice = 100,
            DepositPaid = false,
            BookingDates = new BookingDatesDto
            {
                CheckIn = "2099-02-01",
                CheckOut = "2099-02-05"
            }
        };

        var createContent = new StringContent(
            JsonConvert.SerializeObject(booking),
            Encoding.UTF8,
            "application/json");

        var createResponse = await _fixture.HttpClient.PostAsync("/booking", createContent);
        var createBody = await createResponse.Content.ReadAsStringAsync();
        var created = JsonConvert.DeserializeObject<CreateBookingResponseDto>(createBody);

        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        created.Should().NotBeNull();

        // Act
        var response = await _fixture.HttpClient.GetAsync($"/booking?lastname={uniqueLastName}");
        var responseBody = await response.Content.ReadAsStringAsync();
        var bookingIds = JsonConvert.DeserializeObject<List<BookingIdResponseDto>>(responseBody);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        bookingIds.Should().NotBeNull();
        bookingIds!.Should().Contain(b => b.BookingId == created!.BookingId);
    }

    [Fact]
    public async Task GET_booking_FilterByCheckoutDate_ShouldReturnMatchingBookings()
    {
        // Arrange - Create a booking with a far-future checkout date
        var uniqueCheckout = "2099-12-20";
        var booking = new BookingDto
        {
            FirstName = "CheckoutFilter",
            LastName = "Test",
            TotalPrice = 100,
            DepositPaid = false,
            BookingDates = new BookingDatesDto
            {
                CheckIn = "2099-12-15",
                CheckOut = uniqueCheckout
            }
        };

        var createContent = new StringContent(
            JsonConvert.SerializeObject(booking),
            Encoding.UTF8,
            "application/json");

        var createResponse = await _fixture.HttpClient.PostAsync("/booking", createContent);
        var createBody = await createResponse.Content.ReadAsStringAsync();
        var created = JsonConvert.DeserializeObject<CreateBookingResponseDto>(createBody);

        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        created.Should().NotBeNull();

        // Act - filter by checkout date (returns bookings with checkout >= date)
        var response = await _fixture.HttpClient.GetAsync($"/booking?checkout={uniqueCheckout}");
        var responseBody = await response.Content.ReadAsStringAsync();
        var bookingIds = JsonConvert.DeserializeObject<List<BookingIdResponseDto>>(responseBody);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        bookingIds.Should().NotBeNull();
        bookingIds!.Should().Contain(b => b.BookingId == created!.BookingId);
    }

    [Fact]
    public async Task GET_booking_WithValidId_ShouldReturnAllFields()
    {
        // Arrange - Create a booking with all fields populated
        var booking = new BookingDto
        {
            FirstName = "AllFields",
            LastName = "Validation",
            TotalPrice = 350,
            DepositPaid = true,
            BookingDates = new BookingDatesDto
            {
                CheckIn = "2025-06-01",
                CheckOut = "2025-06-07"
            },
            AdditionalNeeds = "Late checkout"
        };

        var createContent = new StringContent(
            JsonConvert.SerializeObject(booking),
            Encoding.UTF8,
            "application/json");

        var createResponse = await _fixture.HttpClient.PostAsync("/booking", createContent);
        var createBody = await createResponse.Content.ReadAsStringAsync();
        var created = JsonConvert.DeserializeObject<CreateBookingResponseDto>(createBody);

        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        created.Should().NotBeNull();

        // Act
        var response = await _fixture.HttpClient.GetAsync($"/booking/{created!.BookingId}");
        var responseBody = await response.Content.ReadAsStringAsync();
        var retrievedBooking = JsonConvert.DeserializeObject<BookingDto>(responseBody);

        // Assert - verify all fields are present and correct
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        retrievedBooking.Should().NotBeNull();
        retrievedBooking!.FirstName.Should().Be(booking.FirstName);
        retrievedBooking.LastName.Should().Be(booking.LastName);
        retrievedBooking.TotalPrice.Should().Be(booking.TotalPrice);
        retrievedBooking.DepositPaid.Should().Be(booking.DepositPaid);
        retrievedBooking.BookingDates.Should().NotBeNull();
        retrievedBooking.BookingDates.CheckIn.Should().Be(booking.BookingDates.CheckIn);
        retrievedBooking.BookingDates.CheckOut.Should().Be(booking.BookingDates.CheckOut);
        retrievedBooking.AdditionalNeeds.Should().Be(booking.AdditionalNeeds);
    }

    [Fact]
    public async Task POST_booking_WithoutAdditionalNeeds_ShouldCreateBooking()
    {
        // Arrange - additionalneeds is optional per the API spec
        var booking = new BookingDto
        {
            FirstName = "NoNeeds",
            LastName = "Test",
            TotalPrice = 120,
            DepositPaid = false,
            BookingDates = new BookingDatesDto
            {
                CheckIn = "2025-03-01",
                CheckOut = "2025-03-03"
            }
            // AdditionalNeeds intentionally omitted
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
    }

    [Fact]
    public async Task PUT_booking_WithoutAuth_ShouldReturn403Forbidden()
    {
        // Arrange - Create a booking first
        var originalBooking = new BookingDto
        {
            FirstName = "NoAuthPut",
            LastName = "Test",
            TotalPrice = 100,
            DepositPaid = false,
            BookingDates = new BookingDatesDto
            {
                CheckIn = "2025-04-01",
                CheckOut = "2025-04-05"
            }
        };

        var createContent = new StringContent(
            JsonConvert.SerializeObject(originalBooking),
            Encoding.UTF8,
            "application/json");

        var createResponse = await _fixture.HttpClient.PostAsync("/booking", createContent);
        var createBody = await createResponse.Content.ReadAsStringAsync();
        var created = JsonConvert.DeserializeObject<CreateBookingResponseDto>(createBody);

        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        created.Should().NotBeNull();

        var updatedBooking = new BookingDto
        {
            FirstName = "ShouldFail",
            LastName = "Test",
            TotalPrice = 200,
            DepositPaid = true,
            BookingDates = new BookingDatesDto
            {
                CheckIn = "2025-04-01",
                CheckOut = "2025-04-10"
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
        // No auth header intentionally

        // Act
        var response = await _fixture.HttpClient.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task PATCH_booking_WithPartialData_ShouldUpdateOnlyProvidedFields()
    {
        // Arrange - Create a booking first
        var originalBooking = new BookingDto
        {
            FirstName = "PatchFirst",
            LastName = "PatchLast",
            TotalPrice = 500,
            DepositPaid = true,
            BookingDates = new BookingDatesDto
            {
                CheckIn = "2025-07-01",
                CheckOut = "2025-07-05"
            }
        };

        var createContent = new StringContent(
            JsonConvert.SerializeObject(originalBooking),
            Encoding.UTF8,
            "application/json");

        var createResponse = await _fixture.HttpClient.PostAsync("/booking", createContent);
        var createBody = await createResponse.Content.ReadAsStringAsync();
        var created = JsonConvert.DeserializeObject<CreateBookingResponseDto>(createBody);

        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        created.Should().NotBeNull();

        // Get auth token
        var token = await _fixture.GetAuthTokenAsync();

        // Partial payload - only updating firstname and lastname
        var partialUpdate = new { firstname = "PatchedFirst", lastname = "PatchedLast" };
        var patchContent = new StringContent(
            JsonConvert.SerializeObject(partialUpdate),
            Encoding.UTF8,
            "application/json");

        var request = new HttpRequestMessage(HttpMethod.Patch, $"/booking/{created!.BookingId}")
        {
            Content = patchContent
        };
        request.Headers.Add("Cookie", $"token={token}");

        // Act
        var patchResponse = await _fixture.HttpClient.SendAsync(request);
        var patchBody = await patchResponse.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BookingDto>(patchBody);

        // Assert - patched fields changed, unpatched fields retained
        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.FirstName.Should().Be("PatchedFirst");
        result.LastName.Should().Be("PatchedLast");
        result.TotalPrice.Should().Be(originalBooking.TotalPrice);
        result.DepositPaid.Should().Be(originalBooking.DepositPaid);
    }

    [Fact]
    public async Task PATCH_booking_WithoutAuth_ShouldReturn403Forbidden()
    {
        // Arrange - Create a booking first
        var booking = new BookingDto
        {
            FirstName = "PatchNoAuth",
            LastName = "Test",
            TotalPrice = 100,
            DepositPaid = false,
            BookingDates = new BookingDatesDto
            {
                CheckIn = "2025-08-01",
                CheckOut = "2025-08-03"
            }
        };

        var createContent = new StringContent(
            JsonConvert.SerializeObject(booking),
            Encoding.UTF8,
            "application/json");

        var createResponse = await _fixture.HttpClient.PostAsync("/booking", createContent);
        var createBody = await createResponse.Content.ReadAsStringAsync();
        var created = JsonConvert.DeserializeObject<CreateBookingResponseDto>(createBody);

        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        created.Should().NotBeNull();

        var partialUpdate = new { firstname = "ShouldFail" };
        var patchContent = new StringContent(
            JsonConvert.SerializeObject(partialUpdate),
            Encoding.UTF8,
            "application/json");

        var request = new HttpRequestMessage(HttpMethod.Patch, $"/booking/{created!.BookingId}")
        {
            Content = patchContent
        };
        // No auth header intentionally

        // Act
        var response = await _fixture.HttpClient.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DELETE_booking_WithoutAuth_ShouldReturn403Forbidden()
    {
        // Arrange - Create a booking first
        var booking = new BookingDto
        {
            FirstName = "DeleteNoAuth",
            LastName = "Test",
            TotalPrice = 50,
            DepositPaid = false,
            BookingDates = new BookingDatesDto
            {
                CheckIn = "2025-09-01",
                CheckOut = "2025-09-02"
            }
        };

        var createContent = new StringContent(
            JsonConvert.SerializeObject(booking),
            Encoding.UTF8,
            "application/json");

        var createResponse = await _fixture.HttpClient.PostAsync("/booking", createContent);
        var createBody = await createResponse.Content.ReadAsStringAsync();
        var created = JsonConvert.DeserializeObject<CreateBookingResponseDto>(createBody);

        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        created.Should().NotBeNull();

        // Act - DELETE with no auth header
        var response = await _fixture.HttpClient.DeleteAsync($"/booking/{created!.BookingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DELETE_booking_ShouldReturnNotFoundAfterDeletion()
    {
        // Arrange - Create a booking first
        var booking = new BookingDto
        {
            FirstName = "DeleteVerify",
            LastName = "Test",
            TotalPrice = 50,
            DepositPaid = false,
            BookingDates = new BookingDatesDto
            {
                CheckIn = "2025-10-01",
                CheckOut = "2025-10-02"
            }
        };

        var createContent = new StringContent(
            JsonConvert.SerializeObject(booking),
            Encoding.UTF8,
            "application/json");

        var createResponse = await _fixture.HttpClient.PostAsync("/booking", createContent);
        var createBody = await createResponse.Content.ReadAsStringAsync();
        var created = JsonConvert.DeserializeObject<CreateBookingResponseDto>(createBody);

        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        created.Should().NotBeNull();

        var token = await _fixture.GetAuthTokenAsync();
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"/booking/{created!.BookingId}");
        deleteRequest.Headers.Add("Cookie", $"token={token}");
        var deleteResponse = await _fixture.HttpClient.SendAsync(deleteRequest);
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Act - Attempt to retrieve the deleted booking
        var getResponse = await _fixture.HttpClient.GetAsync($"/booking/{created.BookingId}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}