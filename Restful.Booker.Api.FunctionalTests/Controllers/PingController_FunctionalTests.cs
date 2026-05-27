using FluentAssertions;
using Restful.Booker.Api.Tests.Fixtures;
using System.Net;

namespace Restful.Booker.Api.Tests;

public class PingController_FunctionalTests : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;

    public PingController_FunctionalTests(TestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GET_ping_ShouldReturn201Created()
    {
        // Act
        var response = await _fixture.HttpClient.GetAsync("/ping");

        // Assert - API returns 201 Created for a successful health check
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}