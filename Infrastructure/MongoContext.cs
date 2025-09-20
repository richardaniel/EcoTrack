using Ecotrack_Api.Config;
using Ecotrack_Api.Domain;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Ecotrack_Api.Infrastructure;

public interface IMongoContext
{
    IMongoDatabase Database { get; }
    IMongoCollection<User> Users { get; }
    IMongoCollection<Organization> Organizations { get; }
    IMongoCollection<ElectricityConsumption> ElectricityConsumptions { get; }
    IMongoCollection<FuelConsumption> FuelConsumptions { get; }
    IMongoCollection<EnergyConsumption> EnergyConsumptions { get; }
}

public class MongoContext : IMongoContext
{
    public IMongoDatabase Database { get; }

    public MongoContext(IOptions<MongoOptions> options)
    {
        var cs = options.Value.ConnectionString?.Trim();
        var dbName = options.Value.Database?.Trim();
        if (string.IsNullOrWhiteSpace(cs))
            throw new InvalidOperationException("Mongo:ConnectionString vacío en appsettings.");

        try
        {
            // Usa MongoUrl para validar bien el formato
            var url = new MongoUrl(cs);
            var client = new MongoClient(url);
            var databaseName = string.IsNullOrWhiteSpace(dbName)
                ? (url.DatabaseName ?? "Ecotrack")
                : dbName;

            Database = client.GetDatabase(databaseName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ MongoContext: cadena inválida o no conectó. Detalle: {ex.Message}");
            throw; // deja que la app lo muestre como 500 si aún falla
        }
    }

    public IMongoCollection<User> Users => Database.GetCollection<User>("users");
    public IMongoCollection<Organization> Organizations => Database.GetCollection<Organization>("organizations");
    public IMongoCollection<ElectricityConsumption> ElectricityConsumptions => Database.GetCollection<ElectricityConsumption>("electricity_consumptions");
    public IMongoCollection<FuelConsumption> FuelConsumptions => Database.GetCollection<FuelConsumption>("fuel_consumptions");
    public IMongoCollection<EnergyConsumption> EnergyConsumptions => Database.GetCollection<EnergyConsumption>("energy_consumptions");
}
