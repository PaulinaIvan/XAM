using System.Net;
using MaxMind.GeoIP2;

namespace XAM.Middleware;

public class CountryFilterMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _blockedCountryCode;
    private readonly DatabaseReader _databaseReader;

    public CountryFilterMiddleware(RequestDelegate next, string blockedCountryCode, DatabaseReader databaseReader)
    {
        _next = next;
        _blockedCountryCode = blockedCountryCode;
        _databaseReader = databaseReader;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress;

        try
        {
            if (ipAddress == null || IPAddress.IsLoopback(ipAddress) || ipAddress.Equals(IPAddress.IPv6Any) || ipAddress.Equals(IPAddress.IPv6None))
            {
                await _next(context);
                return;
            }

            var response = _databaseReader.Country(ipAddress);

            if (response.Country.IsoCode != _blockedCountryCode)
            {
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync($"Access denied. Requests from {_blockedCountryCode} are not allowed.");
            }
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync($"Internal server error: {ex}");
        }
    }
}

public static class CountryFilterMiddlewareExtensions
{
    public static IApplicationBuilder UseCountryFilter(this IApplicationBuilder builder, string blockedCountryCode)
    {
        return builder.UseMiddleware<CountryFilterMiddleware>(blockedCountryCode);
    }
}
