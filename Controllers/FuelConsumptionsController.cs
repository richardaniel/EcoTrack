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
public class FuelConsumptionsController : ControllerBase
{
    private readonly IMongoContext _ctx;
    public FuelConsumptionsController(IMongoContext ctx) => _ctx = ctx;

    // DTO con nombre único para Swagger
    public record CreateFuelDto(
        string OrganizationId,
        int Year,
        int Month,
        string FuelType,
        decimal Quantity,
        string Unit,
        decimal FactorValue,
        int? FactorYear,
        string FactorSource = "IPCC",
        string FactorUnit = "kgCO2e/unit"
    );

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFuelDto dto)
    {
        var ef = new EmissionFactor { Value = dto.FactorValue, Year = dto.FactorYear, Source = dto.FactorSource, Unit = dto.FactorUnit };
        var co2e = dto.Quantity * ef.Value;

        var doc = new FuelConsumption
        {
            OrganizationId = dto.OrganizationId,
            Period = new Period { Year = dto.Year, Month = dto.Month },
            FuelType = dto.FuelType,
            Quantity = dto.Quantity,
            Unit = dto.Unit,
            EmissionFactor = ef,
            Co2e = co2e,
            Scope = "scope1",
            Source = "manual"
        };

        await _ctx.FuelConsumptions.InsertOneAsync(doc);
        return CreatedAtAction(nameof(GetById), new { id = doc.Id }, doc);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var item = await _ctx.FuelConsumptions.Find(x => x.Id == id).FirstOrDefaultAsync();
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string organizationId, [FromQuery] int? year, [FromQuery] int? month)
    {
        var filter = Builders<FuelConsumption>.Filter.Eq(x => x.OrganizationId, organizationId);
        if (year.HasValue) filter &= Builders<FuelConsumption>.Filter.Eq(x => x.Period.Year, year.Value);
        if (month.HasValue) filter &= Builders<FuelConsumption>.Filter.Eq(x => x.Period.Month, month.Value);

        var items = await _ctx.FuelConsumptions.Find(filter).ToListAsync();
        return Ok(items);
    }
}
