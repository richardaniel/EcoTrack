using Ecotrack_Api.Domain;
using Ecotrack_Api.Domain.Common;
using Ecotrack_Api.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Ecotrack_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ElectricityConsumptionsController : ControllerBase
{
    private readonly IMongoContext _ctx;
    public ElectricityConsumptionsController(IMongoContext ctx) => _ctx = ctx;

    // DTO con nombre único para Swagger
    public record CreateElectricityDto(
        string OrganizationId,
        int Year,
        int Month,
        string? MeterId,
        decimal kWh,
        string? GridRegion,
        decimal FactorValue,
        int? FactorYear,
        string FactorSource = "Operador local",
        string FactorUnit = "kgCO2e/kWh"
    );

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateElectricityDto dto)
    {
        var ef = new EmissionFactor { Value = dto.FactorValue, Year = dto.FactorYear, Source = dto.FactorSource, Unit = dto.FactorUnit };
        var co2e = dto.kWh * ef.Value;

        var doc = new ElectricityConsumption
        {
            OrganizationId = dto.OrganizationId,
            Period = new Period { Year = dto.Year, Month = dto.Month },
            MeterId = dto.MeterId,
            kWh = dto.kWh,
            GridRegion = dto.GridRegion,
            EmissionFactor = ef,
            Co2e = co2e,
            Scope = "scope2",
            Source = "manual"
        };

        await _ctx.ElectricityConsumptions.InsertOneAsync(doc);
        return CreatedAtAction(nameof(GetById), new { id = doc.Id }, doc);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var item = await _ctx.ElectricityConsumptions.Find(x => x.Id == id).FirstOrDefaultAsync();
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string organizationId, [FromQuery] int? year, [FromQuery] int? month)
    {
        var filter = Builders<ElectricityConsumption>.Filter.Eq(x => x.OrganizationId, organizationId);
        if (year.HasValue) filter &= Builders<ElectricityConsumption>.Filter.Eq(x => x.Period.Year, year.Value);
        if (month.HasValue) filter &= Builders<ElectricityConsumption>.Filter.Eq(x => x.Period.Month, month.Value);

        var items = await _ctx.ElectricityConsumptions.Find(filter).ToListAsync();
        return Ok(items);
    }
}
