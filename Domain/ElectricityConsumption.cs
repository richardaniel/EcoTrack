using Ecotrack_Api.Domain.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace Ecotrack_Api.Domain;


public class ElectricityConsumption : BaseEntity
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string OrganizationId { get; set; } = string.Empty;
    public Period Period { get; set; } = new();
    public string? MeterId { get; set; }
    public decimal kWh { get; set; }
    public string? GridRegion { get; set; }
    public EmissionFactor EmissionFactor { get; set; } = new();
    public decimal Co2e { get; set; }
    public string Scope { get; set; } = "scope2";
    public string Source { get; set; } = "manual"; // manual | receipt | iot


    [BsonRepresentation(BsonType.ObjectId)]
    public string? ReceiptId { get; set; }
}
