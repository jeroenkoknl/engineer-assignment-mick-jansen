using System.Text.Json;

namespace SwapfietsApi.Services;

public class FleetTelemetryClient
{
    private const string BaseUrl = "https://fleet-telemetry.swapfiets.internal/api";

    public BikeLocation? GetLocation(int bikeId)
    {
        using var client = new HttpClient();
        var json = client.GetStringAsync($"{BaseUrl}/bikes/{bikeId}/location").Result;
        return JsonSerializer.Deserialize<BikeLocation>(json);
    }
}

public record BikeLocation
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
}
