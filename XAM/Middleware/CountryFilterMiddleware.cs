using System.Net;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Exceptions;
using MaxMind.GeoIP2.Responses;

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
            // Uncomment block to test
            /*
            if (!context.Request.Path.StartsWithSegments("/Home/Denied"))
            {
                context.Response.Redirect($"/Home/Denied?blockedCountryCode={_blockedCountryCode}");
            }
            await _next(context);
            return;
            */
            if (ipAddress == null || IPAddress.IsLoopback(ipAddress))
            {
                await _next(context);
                return;
            }

            CountryResponse response;
            try
            {
                response = _databaseReader.Country(ipAddress);
            }
            catch (AddressNotFoundException)
            {
                Console.WriteLine($"Address {ipAddress} not found in GeoIP2.");
                await _next(context);
                return;
            }

            if (response.Country.IsoCode == _blockedCountryCode && !context.Request.Path.StartsWithSegments("/Home/Denied"))
            {
                context.Response.Redirect($"/Home/Denied?blockedCountryCode={_blockedCountryCode}");
            }
            await _next(context);
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
