using Microsoft.Extensions.Configuration;

namespace Restful.Booker.Api.Tests.Configuration;

public static class TestConfiguration
{
    private static IConfiguration? _configuration;

    public static IConfiguration Configuration
    {
        get
        {
            if (_configuration == null)
            {
                _configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true)
                    .AddUserSecrets<ApiSettings>()
                    .AddEnvironmentVariables()
                    .Build();
            }
            return _configuration;
        }
    }

    public static ApiSettings GetApiSettings()
    {
        var settings = new ApiSettings();
        Configuration.GetSection("ApiSettings").Bind(settings);
        return settings;
    }

}