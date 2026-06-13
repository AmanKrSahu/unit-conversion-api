# API Reference & Testing Guide

This guide details the available HTTP endpoints and explains how to run and write tests for the Unit Conversion Web API.

---

## API Documentation

The API runs by default on port `5000` (HTTP) and `5001` (HTTPS). When running in Development mode, you can view the interactive documentation at:
`http://localhost:5000/swagger/index.html`

### 1. Convert Units
Perform numerical value conversion between two units within the same category.

* **Endpoint**: `GET /api/convert`
* **Query Parameters**:
  * `value` (double, required): The numerical value to convert. Must be a finite number.
  * `from` (string, required): The source unit name (case-insensitive, e.g., `meters`, `celsius`, `kg`).
  * `to` (string, required): The target unit name (case-insensitive, e.g., `feet`, `fahrenheit`, `pounds`).
* **Headers**: `Accept: application/json`

#### Example Request:
```bash
curl -X GET "http://localhost:5000/api/convert?value=100&from=celsius&to=fahrenheit"
```

#### Successful Response (`200 OK`):
```json
{
  "originalValue": 100,
  "fromUnit": "celsius",
  "convertedValue": 212,
  "toUnit": "fahrenheit",
  "category": "Temperature"
}
```

#### Validation Error Response (`400 Bad Request`):
Occurs if a parameter is empty or if the value is not finite (e.g. `NaN`, `Infinity`).
```json
{
  "error": "The 'value' must be a finite number."
}
```

#### Unsupported Unit Response (`404 Not Found`):
Occurs if the input unit is unrecognized or not loaded in the registry.
```json
{
  "error": "Unit 'invalid_unit' is not supported."
}
```

#### Incompatible Category Response (`400 Bad Request`):
Occurs if you attempt to convert between different unit types (e.g. Length to Weight).
```json
{
  "error": "Cannot convert between different categories: 'Length' and 'Weight'."
}
```

---

### 2. Supported Categories
List all unit categories supported by the engine.

* **Endpoint**: `GET /api/categories`

#### Example Request:
```bash
curl -X GET "http://localhost:5000/api/categories"
```

#### Successful Response (`200 OK`):
```json
[
  "Temperature",
  "Length",
  "Weight"
]
```

---

### 3. Supported Units
List all registered units, optionally filtered by a specific category.

* **Endpoint**: `GET /api/units`
* **Query Parameters**:
  * `category` (string, optional): The case-insensitive category name (e.g. `Length`, `Weight`, `Temperature`). If omitted, returns all units in the registry.

#### Example Request (Filtered):
```bash
curl -X GET "http://localhost:5000/api/units?category=Length"
```

#### Example Request (All Units):
```bash
curl -X GET "http://localhost:5000/api/units"
```

#### Successful Response (`200 OK`):
```json
[
  "m",
  "meters",
  "meter",
  "cm",
  "centimeters",
  "centimeter",
  "mm",
  "millimeters",
  "kilometer",
  "kilometers",
  "km",
  "inch",
  "inches",
  "in",
  "feet",
  "foot",
  "ft",
  "yard",
  "yards",
  "yd",
  "mile",
  "miles",
  "mi"
]
```

---

### 4. Health Check
Monitor the running status of the API service, returning detailed system metrics and dependency checks.

* **Endpoint**: `GET /api/health`

#### Example Request:
```bash
curl -X GET "http://localhost:5000/api/health"
```

#### Successful Response (`200 OK`):
```json
{
  "Status": "Healthy",
  "TotalDurationMs": 2.54,
  "Entries": {
    "unit_registry": {
      "Status": "Healthy",
      "Description": "Unit Registry is loaded and ready.",
      "Data": {
        "totalUnits": 50,
        "totalCategories": 3
      }
    }
  },
  "SystemMetrics": {
    "Uptime": "00.00:05:12",
    "MemoryUsageMb": 34.56,
    "OperatingSystem": "Microsoft Windows 11 Pro 10.0.22631 Build 22631"
  }
}
```

---

## Testing Guide

The testing project uses **xUnit** and organizes tests into two categories: isolated Unit Tests and full-stack Integration Tests.

### How to Run Tests
From the root directory, execute:
```powershell
dotnet test
```

### 1. Unit Tests
* **Location**: [ConversionServiceTests.cs](../tests/UnitConversion.UnitTests/ConversionServiceTests.cs)
* **Purpose**: Test the mathematical calculations, offset formulas, validations, and edge cases in isolation from Kestrel, Controllers, or Middleware.
* **Coverage**:
  * Checks standard linear scales (meters to feet, grams to kilograms).
  * Checks complex non-linear scales (Celsius/Fahrenheit/Kelvin conversions).
  * Validates exception throwing (`UnsupportedUnitException`, `InvalidConversionException`, `ArgumentException`).
  * Asserts the same-unit short-circuit optimization.

### 2. Integration Tests
* **Location**: [ConvertApiIntegrationTests.cs](../tests/UnitConversion.IntegrationTests/ConvertApiIntegrationTests.cs)
* **Purpose**: Spin up the complete ASP.NET Core API server in-memory using `WebApplicationFactory` to verify HTTP behaviors, parameters binding, and global middleware handler outputs.
* **Coverage**:
  * Requests valid conversions and checks that HTTP `200 OK` returns correct JSON schemas.
  * Submits invalid values (e.g. `NaN`) and expects HTTP `400 BadRequest`.
  * Submits empty parameters and expects HTTP `400 BadRequest`.
  * Submits unknown unit strings and expects HTTP `404 NotFound`.
