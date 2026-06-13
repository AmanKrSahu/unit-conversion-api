# Unit Conversion Web API

A clean, extensible, and high-performance ASP.NET Core RESTful Web API designed to convert numerical values between different units of measurement across multiple categories (Length, Weight/Mass, Temperature).

This repository represents a production-grade C# project structure applying enterprise architecture patterns, separation of concerns, and clean testing methodologies.

---

## Technical Stack & Architecture

- **Runtime**: .NET 8.0 (configured to roll forward to newer runtimes like .NET 10 automatically)
- **Framework**: ASP.NET Core Web API
- **Testing**: xUnit with `Microsoft.AspNetCore.Mvc.Testing`
- **Design Paradigms**: Clean Architecture, Strategy Pattern, and Registry Pattern.

Production source code and testing codes are organized in a standard enterprise folder layout:
```text
/
├── docs/                               # Technical documentation files
├── src/                                # Production source code projects
│   ├── UnitConversion.Domain/          # Core logic (Registry, Converters, Services)
│   └── UnitConversion.Api/             # Web endpoints, controller, and middleware
├── tests/                              # Unit and Integration test projects
│   ├── UnitConversion.UnitTests/       # Unit testing suite
│   └── UnitConversion.IntegrationTests/# Integration testing suite
├── .gitignore                          # Files excluded from git tracking
└── UnitConversion.sln                  # Visual Studio solution file
```

> [!NOTE]
> Detailed information about architectural layers, structural logic, and class files can be found in the [Architecture Documentation](docs/architecture.md).

---

## Getting Started

### Prerequisites
To build and run this project, you need the **.NET 8.0 SDK** (or later) installed on your machine.
Verify your installation by running:
```powershell
dotnet --version
```

### Running the Web API
1. Clone the repository and navigate to the project root:
   ```powershell
   git clone https://github.com/AmanKrSahu/unit-conversion-api.git
   cd unit-conversion-api
   ```
2. Restore package dependencies:
   ```powershell
   dotnet restore
   ```
3. Build the solution:
   ```powershell
   dotnet build
   ```
4. Run the API:
   ```powershell
   dotnet run --project src/UnitConversion.Api
   ```
By default, the API will start and listen on:
- HTTP: `http://localhost:5000`
- Swagger UI (Development mode): `http://localhost:5000/swagger/index.html`

---

## API Reference & Testing

We provide HTTP REST endpoints to perform calculations and retrieve supported metadata. In addition, we maintain a robust test suite verifying both execution logic (Unit Tests) and server host routing (Integration Tests).

For detailed API requests, JSON response schemas, and instructions on executing tests, please refer to the [API & Testing Guide](docs/api_testing_guide.md).

Quick test command:
```powershell
dotnet test
```

---

## Best Engineering Practices Followed

1. **Decoupled Business Logic**: No dependencies on ASP.NET Core or third-party web helpers inside the `Domain` project. Core math is entirely portable.
2. **Registry Decoupling**: Converter classes fetch unit definitions (scale factors) dynamically from `IUnitRegistry` instead of hardcoding factors inside converter bodies.
3. **Robust Request Validation**: Validates all input query arguments against invalid entries (e.g. `NaN`, `Infinity`, empty strings) before processing.
4. **Global Exception Mapping**: Replaced raw tracebacks with custom domain exceptions mapping automatically to standard HTTP response codes (`400 Bad Request` and `404 Not Found`) via custom middleware.
5. **No Build Artifacts Commits**: Included a root `.gitignore` file to filter out temporary compiling cache (`bin/` and `obj/`) and IDE specific user layouts.

---

## Future Scalability (How to Proceed)

This project is specifically designed to support scaling to thousands of units and hot-reload definitions:

1. **Add new categories**: If linear (e.g. Volume), add unit definitions to the registry and register a new converter instance in `Program.cs`. If non-linear, implement `IUnitConverter` and register.
2. **Database Persistence**: To scale, swap the in-memory `UnitRegistry` implementation with a database repository (`DbUnitRegistry : IUnitRegistry`) powered by Entity Framework Core or Dapper. This will let you manage, insert, and update units via a database or administration UI without redeploying code.
3. **Validation pipeline**: Scale DTO validation to use FluentValidation pipeline middleware filters for complex, localized error messages.
