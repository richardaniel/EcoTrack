using Ecotrack_Api.Domain.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace Ecotrack_Api.Domain;


public class EnergyConsumption : BaseEntity
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string OrganizationId { get; set; } = string.Empty;
    public Period Period { get; set; } = new();


    public string EnergyType { get; set; } = string.Empty; // gas_natural, vapor, etc.
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = "kg"; // kg | t | m3 | kWh_thermal | BTU | otro


    public EmissionFactor EmissionFactor { get; set; } = new();
    public decimal Co2e { get; set; }
    public string Scope { get; set; } = "scope1"; // scope1 | scope2 | scope3
    public string Source { get; set; } = "manual"; // manual | receipt | iot


    [BsonRepresentation(BsonType.ObjectId)]
    public string? ReceiptId { get; set; }
}