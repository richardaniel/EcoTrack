using Ecotrack_Api.Domain.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace Ecotrack_Api.Domain;


public class FuelConsumption : BaseEntity
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string OrganizationId { get; set; } = string.Empty;
    public Period Period { get; set; } = new();


    public string FuelType { get; set; } = string.Empty; // gasolina, diésel, etc.
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = "L"; // L | gal | kg | m3


    public EmissionFactor EmissionFactor { get; set; } = new();
    public decimal Co2e { get; set; }
    public string Scope { get; set; } = "scope1";
    public string Source { get; set; } = "manual"; // manual | receipt | telematics


    [BsonRepresentation(BsonType.ObjectId)]
    public string? ReceiptId { get; set; }
}