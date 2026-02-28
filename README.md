# CsvToolkit — OutSystems External Library

A .NET 8 external library for OutSystems Developer Cloud (ODC) that provides actions for parsing, converting, validating, and inspecting CSV data.

Built on [CsvHelper](https://joshclose.github.io/CsvHelper/) v33.

## Available Actions

| Action | Description |
|---|---|
| **CsvToJson** | Parses CSV text and converts it to a JSON array string. Use JSON Deserialize in OutSystems to convert the result to a Record List. |
| **JsonToCsv** | Converts a JSON array string to CSV text for export or download. Use JSON Serialize on a Record List to generate the input. |
| **GetCsvHeaders** | Extracts column header names and positions from CSV content. Useful for inspecting structure before conversion. |
| **CountCsvRecords** | Counts the number of data records in CSV content (excludes the header row if present). |
| **ValidateCsv** | Validates CSV structure for consistent column counts across all rows and returns any issues found. |

## Structures

| Structure | Fields | Description |
|---|---|---|
| **CsvColumnInfo** | `Name`, `Index` | Represents a column header name and its position. |
| **CsvValidationResult** | `IsValid`, `RecordCount`, `ColumnCount`, `ErrorDetails` | Result of a CSV structure validation. |

## Supported Features

- Custom delimiters (comma, semicolon, tab, or any string)
- Header row detection and auto-generated column names (`Column1`, `Column2`, etc.)
- Quoted fields with embedded commas and escaped quotes
- Consistent error handling via `IsSuccess` and `ErrorMessage` output parameters

## Installation

1. Download the latest `CsvToolkit_v*.zip` from [Releases](../../releases).
2. In the ODC Portal, go to **External Libraries** and upload the zip file.
3. The **CsvToolkit** library and its actions will appear in ODC Studio for use in your applications.

## Building from Source

```bash
dotnet restore
dotnet build -c Release
dotnet test -c Release
```

## Running Tests

```bash
dotnet test OutSystems.Extension.CsvToolkit.Tests/ -c Release --collect:"XPlat Code Coverage"
```

## Creating a Release Package

Tag a version to trigger the GitHub Actions release workflow:

```bash
git tag v1.0.0
git push origin v1.0.0
```

This runs tests, builds, and publishes a `CsvToolkit_v1.0.0.zip` as a GitHub Release.
