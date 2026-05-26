using FluentAssertions;
using Newtonsoft.Json;
using Restful.Booker.Api.Tests.Fixtures;
using Restful.Booker.Api.Tests.Models;
using System.Text;

namespace Restful.Booker.Api.Tests;

public class AuthController_FunctionalTests : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;

    public AuthController_FunctionalTests(TestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task POST_auth_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var authRequest = new
        {
            username = _fixture.Settings.Authentication.Username,
            password = _fixture.Settings.Authentication.Password
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(authRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _fixture.HttpClient.PostAsync("/auth", content);
        var responseBody = await response.Content.ReadAsStringAsync();
        var authResponse = JsonConvert.DeserializeObject<AuthResponseDto>(responseBody);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        authResponse.Should().NotBeNull();
        authResponse!.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task POST_auth_InvalidCredentials_Returns200WithReasonMessage()
    {
        // Arrange
        var authRequest = new
        {
            username = "invalid",
            password = "invalid"
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(authRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _fixture.HttpClient.PostAsync("/auth", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert - API returns 200 with a reason field instead of a token
        response.IsSuccessStatusCode.Should().BeTrue();
        responseBody.Should().Contain("reason");
        responseBody.Should().Contain("Bad credentials");
    }

    [Fact]
    public async Task POST_auth_MissingCredentials_Returns200WithReasonMessage()
    {
        // Arrange - send empty body
        var content = new StringContent(
            "{}",
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _fixture.HttpClient.PostAsync("/auth", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert - API returns 200 with a reason field instead of a token
        response.IsSuccessStatusCode.Should().BeTrue();
        responseBody.Should().Contain("reason");
    }
}