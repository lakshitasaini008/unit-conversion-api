using Microsoft.AspNetCore.Mvc;
using UnitConversionApi.Models;
using UnitConversionApi.Services;

namespace UnitConversionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConversionController : ControllerBase
{
    private readonly IConversionService _conversionService;

    public ConversionController(IConversionService conversionService)
    {
        _conversionService = conversionService;
    }

    /// <summary>
    /// Convert a value from one unit to another.
    /// </summary>
    /// <remarks>
    /// Example: POST /api/conversion
    /// {
    ///   "value": 100,
    ///   "fromUnit": "kilometer",
    ///   "toUnit": "mile"
    /// }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ConversionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Convert([FromBody] ConversionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FromUnit) || string.IsNullOrWhiteSpace(request.ToUnit))
            return BadRequest(new { error = "Both 'fromUnit' and 'toUnit' are required." });

        try
        {
            var result = _conversionService.Convert(request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get all supported units grouped by category.
    /// </summary>
    [HttpGet("units")]
    [ProducesResponseType(typeof(IEnumerable<UnitInfo>), StatusCodes.Status200OK)]
    public IActionResult GetUnits()
    {
        var units = _conversionService.GetSupportedUnits();
        return Ok(units);
    }

    /// <summary>
    /// Get all supported conversion categories.
    /// </summary>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public IActionResult GetCategories()
    {
        var categories = _conversionService.GetCategories();
        return Ok(categories);
    }
}
