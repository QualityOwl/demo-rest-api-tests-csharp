# My REST API Tests Project (C#)

A C# test project demonstrating REST API testing against the [Restful Booker](https://restful-booker.herokuapp.com) public API.

## What is tested

Tests cover three areas of the Restful Booker API:

- **Auth** - token generation via `POST /auth`
- **Booking** - create, read, update, and delete bookings via `/booking`
- **Health check** - service availability via `GET /ping`

## Tech stack

- .NET 10 / xUnit
- FluentAssertions
- Newtonsoft.Json
- `HttpClient` via `Microsoft.Extensions.Http`
- `Microsoft.Extensions.Configuration` for settings and secrets

## Configuration

API credentials are read from `Config/appsettings.json` and can be overridden with environment variables (used in CI).

## Running the tests

```bash
dotnet test
```
