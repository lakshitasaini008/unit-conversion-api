# Unit Conversion API

A simple ASP.NET Core Web API that converts numerical values between different units of measurement.

## Supported Categories

| Category    | Example units                                          |
|-------------|--------------------------------------------------------|
| Length      | meter, kilometer, mile, foot, inch, yard, cm, mm       |
| Temperature | celsius, fahrenheit, kelvin                            |
| Weight/Mass | kilogram, gram, pound, ounce, ton, milligram           |
| Volume      | liter, milliliter, gallon, quart, pint, cup, fluid_ounce |
| Speed       | meters_per_second, kilometers_per_hour, miles_per_hour, knot |

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)

### Run locally

```bash
# Clone the repo
git clone <your-repo-url>
cd UnitConversionApi

# Start the API
dotnet run --project UnitConversionApi

# The API will be available at https://localhost:5001 (or http://localhost:5000)
# Swagger UI is served at the root: https://localhost:5001/
```

### Run the tests

```bash
dotnet test
```

## API Endpoints

### `POST /api/conversion`

Convert a value from one unit to another.

**Request body:**
```json
{
  "value": 100,
  "fromUnit": "kilometer",
  "toUnit": "mile"
}
```

**Response:**
```json
{
  "inputValue": 100,
  "fromUnit": "kilometer",
  "outputValue": 62.1371,
  "toUnit": "mile",
  "category": "length"
}
```

### `GET /api/conversion/units`

Returns all supported units with their symbols and categories.

### `GET /api/conversion/categories`

Returns all supported conversion categories.

## Design Decisions

**Single conversion endpoint** — A `POST /api/conversion` with a JSON body keeps the contract simple and easy to extend. Query-string alternatives (e.g. `GET /api/conversion?value=1&from=km&to=mi`) were considered but a request body is cleaner when the number of parameters grows.

**Base-unit factor table** — Each unit stores a single multiplier relative to a base unit (meter, kilogram, liter, m/s). Converting A→B is `value * factorA / factorB`. Temperature is special-cased because it requires an offset in addition to scaling.

**Unit aliases** — The lookup accepts both the full name (`kilometer`) and the symbol (`km`), so callers don't need to know the exact naming convention.

**In-memory data** — The requirement states units can be hardcoded for now. The `ConversionService` is registered as `Scoped`, so swapping in a database-backed implementation later is a one-line change in `Program.cs`.

**No floating-point rounding in storage** — Conversion factors are stored as `double` literals. Results are rounded to 10 decimal places at the output boundary to avoid surfacing floating-point noise to callers.
