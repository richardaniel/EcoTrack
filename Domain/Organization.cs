using Ecotrack_Api.Domain.Common;


namespace Ecotrack_Api.Domain;


public class Organization : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Country { get; set; }
    public string? Sector { get; set; }
    public Settings Settings { get; set; } = new();
}


public class Settings
{
    public string Timezone { get; set; } = "America/Tegucigalpa";
    public string Currency { get; set; } = "HNL";
}