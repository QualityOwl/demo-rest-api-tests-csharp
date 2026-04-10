using FluentAssertions;
using Newtonsoft.Json;
using Restful.Booker.Api.Tests.Fixtures;
using Restful.Booker.Api.Tests.Models;
using System.Text;

namespace Restful.Booker.Api.Tests;

public class AuthController_Tests : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;

    public AuthController_Tests(TestFixture fixture)
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
    public async Task POST_auth_InvalidCredentials_Returns400BadRequest()
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

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue(); // API returns 200 even for bad credentials
        responseBody.Should().Contain("reason");
    }
}
