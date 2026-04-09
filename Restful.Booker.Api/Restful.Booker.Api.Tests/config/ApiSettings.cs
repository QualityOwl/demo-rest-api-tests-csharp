namespace Restful.Booker.Api.Tests.Configuration;

public class ApiSettings
{
    public string BaseUrl { get; set; }
    public Authentication Authentication { get; set; }
    public int Timeout { get; set; }
}

public class Authentication
{
    public string Username { get; set; }
    public string Password {  set; get; }
}
