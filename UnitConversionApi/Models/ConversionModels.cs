namespace UnitConversionApi.Models;

public class ConversionRequest
{
    public double Value { get; set; }
    public string FromUnit { get; set; } = string.Empty;
    public string ToUnit { get; set; } = string.Empty;
}

public class ConversionResult
{
    public double InputValue { get; set; }
    public string FromUnit { get; set; } = string.Empty;
    public double OutputValue { get; set; }
    public string ToUnit { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}

public class UnitInfo
{
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}
