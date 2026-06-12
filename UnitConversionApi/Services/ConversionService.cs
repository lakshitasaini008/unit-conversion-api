using UnitConversionApi.Models;

namespace UnitConversionApi.Services;

public interface IConversionService
{
    ConversionResult Convert(ConversionRequest request);
    IEnumerable<UnitInfo> GetSupportedUnits();
    IEnumerable<string> GetCategories();
}

public class ConversionService : IConversionService
{
    // Each unit is stored with a factor to convert TO a base unit.
    // For temperature we use a special-case approach since it's not just a multiply.
    //
    // Base units:
    //   Length   -> meter
    //   Weight   -> kilogram
    //   Temperature -> celsius (handled separately)
    //   Volume   -> liter
    //   Speed    -> meters per second

    private record UnitDefinition(string Name, string Symbol, string Category, double ToBase);

    private static readonly List<UnitDefinition> Units = new()
    {
        // Length
        new("meter",      "m",    "length", 1.0),
        new("kilometer",  "km",   "length", 1000.0),
        new("centimeter", "cm",   "length", 0.01),
        new("millimeter", "mm",   "length", 0.001),
        new("mile",       "mi",   "length", 1609.344),
        new("yard",       "yd",   "length", 0.9144),
        new("foot",       "ft",   "length", 0.3048),
        new("inch",       "in",   "length", 0.0254),

        // Weight / Mass
        new("kilogram",   "kg",   "weight", 1.0),
        new("gram",       "g",    "weight", 0.001),
        new("milligram",  "mg",   "weight", 0.000001),
        new("pound",      "lb",   "weight", 0.453592),
        new("ounce",      "oz",   "weight", 0.0283495),
        new("ton",        "t",    "weight", 1000.0),

        // Volume
        new("liter",      "L",    "volume", 1.0),
        new("milliliter", "mL",   "volume", 0.001),
        new("gallon",     "gal",  "volume", 3.78541),
        new("quart",      "qt",   "volume", 0.946353),
        new("pint",       "pt",   "volume", 0.473176),
        new("cup",        "cup",  "volume", 0.236588),
        new("fluid_ounce","fl oz","volume", 0.0295735),

        // Speed
        new("meters_per_second",   "m/s",  "speed", 1.0),
        new("kilometers_per_hour", "km/h", "speed", 0.277778),
        new("miles_per_hour",      "mph",  "speed", 0.44704),
        new("knot",                "kn",   "speed", 0.514444),

        // Temperature is handled separately — listed here for discovery only
        new("celsius",    "°C",   "temperature", 0),
        new("fahrenheit", "°F",   "temperature", 0),
        new("kelvin",     "K",    "temperature", 0),
    };

    private static readonly HashSet<string> TemperatureUnits =
        new(StringComparer.OrdinalIgnoreCase) { "celsius", "fahrenheit", "kelvin" };

    public ConversionResult Convert(ConversionRequest request)
    {
        var from = request.FromUnit.Trim().ToLowerInvariant();
        var to   = request.ToUnit.Trim().ToLowerInvariant();

        if (TemperatureUnits.Contains(from) || TemperatureUnits.Contains(to))
            return ConvertTemperature(request, from, to);

        var fromDef = FindUnit(from) ?? throw new ArgumentException($"Unknown unit: '{request.FromUnit}'");
        var toDef   = FindUnit(to)   ?? throw new ArgumentException($"Unknown unit: '{request.ToUnit}'");

        if (fromDef.Category != toDef.Category)
            throw new InvalidOperationException(
                $"Cannot convert between '{fromDef.Category}' and '{toDef.Category}'.");

        var baseValue  = request.Value * fromDef.ToBase;
        var result     = baseValue / toDef.ToBase;

        return new ConversionResult
        {
            InputValue  = request.Value,
            FromUnit    = fromDef.Name,
            OutputValue = Math.Round(result, 10),
            ToUnit      = toDef.Name,
            Category    = fromDef.Category,
        };
    }

    public IEnumerable<UnitInfo> GetSupportedUnits() =>
        Units.Select(u => new UnitInfo { Name = u.Name, Symbol = u.Symbol, Category = u.Category });

    public IEnumerable<string> GetCategories() =>
        Units.Select(u => u.Category).Distinct().OrderBy(c => c);

    // -------------------------------------------------------------------------

    private static UnitDefinition? FindUnit(string key) =>
        Units.FirstOrDefault(u =>
            u.Name.Equals(key, StringComparison.OrdinalIgnoreCase) ||
            u.Symbol.Equals(key, StringComparison.OrdinalIgnoreCase));

    private static ConversionResult ConvertTemperature(ConversionRequest request, string from, string to)
    {
        if (!TemperatureUnits.Contains(from)) throw new ArgumentException($"Unknown unit: '{request.FromUnit}'");
        if (!TemperatureUnits.Contains(to))   throw new ArgumentException($"Unknown unit: '{request.ToUnit}'");

        // First convert to Celsius
        double celsius = from switch
        {
            "celsius"    => request.Value,
            "fahrenheit" => (request.Value - 32) * 5 / 9,
            "kelvin"     => request.Value - 273.15,
            _            => throw new ArgumentException($"Unknown temperature unit: '{from}'"),
        };

        double result = to switch
        {
            "celsius"    => celsius,
            "fahrenheit" => celsius * 9 / 5 + 32,
            "kelvin"     => celsius + 273.15,
            _            => throw new ArgumentException($"Unknown temperature unit: '{to}'"),
        };

        return new ConversionResult
        {
            InputValue  = request.Value,
            FromUnit    = from,
            OutputValue = Math.Round(result, 10),
            ToUnit      = to,
            Category    = "temperature",
        };
    }
}
