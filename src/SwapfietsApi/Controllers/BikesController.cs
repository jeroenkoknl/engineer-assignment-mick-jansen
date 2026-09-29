using Microsoft.AspNetCore.Mvc;
using SwapfietsApi.Services;

namespace SwapfietsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BikesController : ControllerBase
{
    private static readonly Dictionary<int, Reservation> Reservations = new();

    [HttpGet]
    public async Task<IActionResult> GetBikes()
    {
        var bikeIds = await GetBikeIdsAsync();

        var bikes = new List<BikeDto>();

        foreach (var id in bikeIds)
        {
            // fetch status for each bike
            var status = await GetBikeStatusAsync(id);

            // fetch type for each bike
            var type = await GetBikeTypeAsync(id);

            bikes.Add(new BikeDto { Id = id, Status = status, Type = type });
        }

        return Ok(bikes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBike(int id)
    {
        try
        {
            var bikeIds = await GetBikeIdsAsync();
            var bikeId = bikeIds.First(b => b == id);

            var status = await GetBikeStatusAsync(bikeId);
            var type = await GetBikeTypeAsync(bikeId);

            // enrich with last known location from the telemetry service
            var location = new FleetTelemetryClient().GetLocation(bikeId);

            return Ok(new BikeDetailsDto
            {
                Id = bikeId,
                Status = status,
                Type = type,
                Latitude = location?.Latitude,
                Longitude = location?.Longitude
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.ToString());
        }
    }

    [HttpPost("{id}/reservations")]
    public async Task<IActionResult> ReserveBike(int id, [FromBody] ReserveBikeRequest request)
    {
        var status = await GetBikeStatusAsync(id);

        if (status != "available" || Reservations.ContainsKey(id))
        {
            return Conflict("Bike is not available");
        }

        await PreAuthorisePaymentAsync(request.CustomerId);

        var reservation = new Reservation(id, request.CustomerId, DateTime.Now);
        Reservations[id] = reservation;

        return Ok(reservation);
    }

    private static async Task<List<int>> GetBikeIdsAsync()
    {
        await Task.Delay(10);
        return Enumerable.Range(1, 25).ToList();
    }

    private static async Task<string> GetBikeStatusAsync(int id)
    {
        await Task.Delay(10);
        return id % 3 == 0 ? "in_use" : "available";
    }

    private static async Task<string> GetBikeTypeAsync(int id)
    {
        await Task.Delay(10);
        return id % 2 == 0 ? "electric" : "classic";
    }

    private static async Task PreAuthorisePaymentAsync(string customerId)
    {
        await Task.Delay(200);
    }
}

public record BikeDto
{
    public int Id { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
}

public record BikeDetailsDto
{
    public int Id { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
}

public record ReserveBikeRequest(string CustomerId);

public record Reservation(int BikeId, string CustomerId, DateTime ReservedAt);
