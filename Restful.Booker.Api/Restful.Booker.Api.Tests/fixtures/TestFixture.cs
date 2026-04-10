using Restful.Booker.Api.Tests.Configuration;
using Restful.Booker.Api.Tests.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Restful.Booker.Api.Tests.Fixtures;

public class TestFixture : IDisposable
{
    public HttpClient HttpClient { get; }
    public ApiSettings Settings { get; }
    private string? _authToken;

    public TestFixture()
    {
        Settings = TestConfiguration.GetApiSettings();

        HttpClient = new HttpClient
        {
            BaseAddress = new Uri(Settings.BaseUrl),
            Timeout = TimeSpan.FromSeconds(Settings.Timeout)
        };

        HttpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<string> GetAuthTokenAsync()
    {
        if (!string.IsNullOrEmpty(_authToken))
            return _authToken;

        var authRequest = new
        {
            username = Settings.Authentication.Username,
            password = Settings.Authentication.Password
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(authRequest),
            Encoding.UTF8,
            "application/json");

        var response = await HttpClient.PostAsync("/auth", content);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var authResponse = JsonConvert.DeserializeObject<AuthResponseDto>(responseBody);

        _authToken = authResponse?.Token ?? throw new InvalidOperationException("Failed to obtain auth token");
        return _authToken;
    }

    public void Dispose()
    {
        HttpClient?.Dispose();
        GC.SuppressFinalize(this);
    }
}
