namespace Ecotrack_Api.Domain.Common;


public class Period
{
    public int Year { get; set; }
    public int Month { get; set; }
}


public class EmissionFactor
{
    public decimal Value { get; set; } // kgCO2e por unidad
    public string Unit { get; set; } = "kgCO2e/kWh";
    public string Source { get; set; } = "Operador local / IPCC";
    public int? Year { get; set; }
}
