<<<<<<< HEAD
# CsvCompareSolution

**CsvCompareSolution** is a small .NET solution that compares CSV files using composite keys and reports differences (missing rows, extra rows, and field-level differences). It contains a reusable library, a console runner, and unit tests.

## Projects

- `CsvComparator` — Core library containing CSV parsing and comparison logic.
- `CsvCompareRunner` — Console application (entry point) to run comparisons from the command line.
- `CsvComparator.Tests` — NUnit test project with unit tests for the comparator.

## Features

- Parse CSV files (handles quoted values and commas inside quotes).
- Compare two CSV files using composite key columns.
- Report:
  - Rows present in expected but missing in actual (`MissingInActual`).
  - Rows present in actual but not expected (`ExtraInActual`).
  - Field-level differences for matching keys (`FieldDifferences`).

## Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- Optional: Visual Studio 2022 or newer / JetBrains Rider for IDE experience

## Build

From the solution root (where `CsvCompareSolution.sln` is located):

```bash
# Restore packages (if any) and build
dotnet build CsvCompareSolution.sln
```

## Run (Console Runner)

The console runner project is `CsvCompareRunner`. Example:

```bash
cd CsvCompareRunner
dotnet run -- expected.csv actual.csv --keys 0 1
```

> Note: The exact CLI argument parsing depends on `Program.cs` implementation. If `Program.cs` expects different arguments, adapt the command accordingly. Sample CSVs (`expected.csv`, `actual.csv`) are included in the repository for quick testing.

## Tests

From solution root run:

```bash
dotnet test
```

This runs the NUnit tests in `CsvComparator.Tests`.

## Project Structure

```
CsvCompareSolution.sln
├── CsvComparator/            # Core library
│   ├── CsvParser.cs
│   ├── CsvComparer.cs
│   ├── CsvRecord.cs
│   └── ComparisonResult.cs
├── CsvCompareRunner/        # Console app (Program.cs)
└── CsvComparator.Tests/     # Unit tests
```

## Usage (high level)

1. Provide two CSV files: an **expected** file and an **actual** file.
2. Choose one or more column indexes to form a composite key that uniquely identifies rows.
3. Run the comparator to get a `ComparisonResult` that includes lists of missing rows, extra rows, and per-field differences.

## Example (pseudo-output)

```json
{
  "MissingInActual": ["key1","key2"],
  "ExtraInActual": ["key3"],
  "FieldDifferences": [
    {"Key":"key4","ColumnIndex":3,"Expected":"foo","Actual":"bar"}
  ]
}
```

## Notes & Next Steps

- Some source files in this extracted copy contain placeholder markers (`...`) indicating parts of the implementation may have been elided. Before relying on the library in production, open `CsvComparator/*.cs` and confirm the full implementation is present.
- Consider adding a richer CLI argument parser (e.g., System.CommandLine or McMaster.Extensions.CommandLineUtils) for better UX.
- Add CSV schema/config support to allow named columns rather than numeric indexes.

## Contributing

Contributions welcome. Please open issues and submit PRs. Follow typical .NET contribution workflows (feature branch, tests, PR description).

## License

Add a license file (e.g., MIT) if you intend to publish this repository.
=======

>>>>>>> 86f8a17c866171c5ca18adb34c9b18e82b361387
