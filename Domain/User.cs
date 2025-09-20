using Ecotrack_Api.Domain.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace Ecotrack_Api.Domain;


public class User : BaseEntity
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string OrganizationId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "viewer"; // admin | analyst | viewer
    public DateTime? LastLoginAt { get; set; }
}