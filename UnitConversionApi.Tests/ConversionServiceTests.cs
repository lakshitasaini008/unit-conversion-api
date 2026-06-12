using UnitConversionApi.Models;
using UnitConversionApi.Services;
using Xunit;

namespace UnitConversionApi.Tests;

public class ConversionServiceTests
{
    private readonly ConversionService _service = new();

    // -------------------------------------------------------------------------
    // Length
    // -------------------------------------------------------------------------

    [Fact]
    public void Convert_MetersToFeet_ReturnsCorrectValue()
    {
        var result = _service.Convert(new ConversionRequest { Value = 1, FromUnit = "meter", ToUnit = "foot" });
        Assert.Equal(3.28084, result.OutputValue, precision: 4);
    }

    [Fact]
    public void Convert_KilometersToMiles_ReturnsCorrectValue()
    {
        var result = _service.Convert(new ConversionRequest { Value = 1, FromUnit = "kilometer", ToUnit = "mile" });
        Assert.Equal(0.621371, result.OutputValue, precision: 4);
    }

    // -------------------------------------------------------------------------
    // Temperature
    // -------------------------------------------------------------------------

    [Fact]
    public void Convert_CelsiusToFahrenheit_ReturnsCorrectValue()
    {
        var result = _service.Convert(new ConversionRequest { Value = 0, FromUnit = "celsius", ToUnit = "fahrenheit" });
        Assert.Equal(32.0, result.OutputValue);
    }

    [Fact]
    public void Convert_FahrenheitToCelsius_ReturnsCorrectValue()
    {
        var result = _service.Convert(new ConversionRequest { Value = 212, FromUnit = "fahrenheit", ToUnit = "celsius" });
        Assert.Equal(100.0, result.OutputValue);
    }

    [Fact]
    public void Convert_CelsiusToKelvin_ReturnsCorrectValue()
    {
        var result = _service.Convert(new ConversionRequest { Value = 0, FromUnit = "celsius", ToUnit = "kelvin" });
        Assert.Equal(273.15, result.OutputValue);
    }

    // -------------------------------------------------------------------------
    // Weight
    // -------------------------------------------------------------------------

    [Fact]
    public void Convert_KilogramsToPounds_ReturnsCorrectValue()
    {
        var result = _service.Convert(new ConversionRequest { Value = 1, FromUnit = "kilogram", ToUnit = "pound" });
        Assert.Equal(2.20462, result.OutputValue, precision: 4);
    }

    // -------------------------------------------------------------------------
    // Error cases
    // -------------------------------------------------------------------------

    [Fact]
    public void Convert_UnknownUnit_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.Convert(new ConversionRequest { Value = 1, FromUnit = "parsec", ToUnit = "meter" }));
    }

    [Fact]
    public void Convert_MismatchedCategories_ThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() =>
            _service.Convert(new ConversionRequest { Value = 1, FromUnit = "meter", ToUnit = "kilogram" }));
    }

    [Fact]
    public void Convert_SameUnit_ReturnsSameValue()
    {
        var result = _service.Convert(new ConversionRequest { Value = 42, FromUnit = "meter", ToUnit = "meter" });
        Assert.Equal(42.0, result.OutputValue);
    }

    // -------------------------------------------------------------------------
    // Discovery endpoints
    // -------------------------------------------------------------------------

    [Fact]
    public void GetSupportedUnits_ReturnsNonEmptyList()
    {
        var units = _service.GetSupportedUnits().ToList();
        Assert.NotEmpty(units);
    }

    [Fact]
    public void GetCategories_IncludesRequiredCategories()
    {
        var categories = _service.GetCategories().ToList();
        Assert.Contains("length", categories);
        Assert.Contains("weight", categories);
        Assert.Contains("temperature", categories);
    }
}
