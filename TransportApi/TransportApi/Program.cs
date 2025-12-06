using Microsoft.EntityFrameworkCore;
using TransportApi.Data;
using TransportApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core + SQLite
builder.Services.AddDbContext<FleetDbContext>(options =>
    options.UseSqlite("Data Source=fleet.db"));

// FleetManager jako serwis
builder.Services.AddScoped<FleetManager>();

var app = builder.Build();

// Swagger UI w dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Tworzenie bazy przy starcie
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FleetDbContext>();
    db.Database.EnsureCreated();
}

// VEHICLES

app.MapGet("/api/vehicles", async (FleetDbContext db) =>
{
    var vehicles = await db.Vehicles.ToListAsync();
    return Results.Ok(vehicles);
});

app.MapPost("/api/vehicles", async (FleetDbContext db, VehicleCreateDto dto) =>
{
    Vehicle vehicle = dto.Type.ToLower() switch
    {
        "truck" => new Truck
        {
            RegistrationNumber = dto.RegistrationNumber,
            Model = dto.Model,
            TrailerLength = dto.TrailerLength ?? 0
        },
        "van" => new Van
        {
            RegistrationNumber = dto.RegistrationNumber,
            Model = dto.Model,
            CargoVolume = dto.CargoVolume ?? 0
        },
        _ => throw new ArgumentException("Unknown vehicle type")
    };

    db.Vehicles.Add(vehicle);
    await db.SaveChangesAsync();
    return Results.Created($"/api/vehicles/{vehicle.Id}", vehicle);
});

// DRIVERS

app.MapGet("/api/drivers", async (FleetDbContext db) =>
{
    var drivers = await db.Drivers.ToListAsync();
    return Results.Ok(drivers);
});

app.MapPost("/api/drivers", async (FleetDbContext db, DriverCreateDto dto) =>
{
    var driver = new Driver
    {
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        IsAvailable = true
    };
    db.Drivers.Add(driver);
    await db.SaveChangesAsync();
    return Results.Created($"/api/drivers/{driver.Id}", driver);
});

// ORDERS

app.MapGet("/api/orders", async (FleetDbContext db) =>
{
    var orders = await db.Orders
        .Include(o => o.Vehicle)
        .Include(o => o.Driver)
        .ToListAsync();

    return Results.Ok(orders);
});

app.MapPost("/api/orders", async (FleetManager fleetManager, OrderCreateDto dto) =>
{
    try
    {
        var order = await fleetManager.CreateOrderAsync(dto.VehicleId, dto.DriverId, dto.CargoDescription);
        return Results.Created($"/api/orders/{order.Id}", order);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPut("/api/orders/{id:int}/complete", async (FleetManager fleetManager, int id) =>
{
    try
    {
        await fleetManager.CompleteOrderAsync(id);
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.Run();
