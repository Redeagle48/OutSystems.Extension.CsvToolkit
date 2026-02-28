using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using OutSystems.Extension.CsvToolkit;

namespace OutSystems.Extension.CsvToolkit.Tests
{
    /// <summary>
    /// Tests for the CsvToJson action: basic CSV parsing, custom delimiters,
    /// and no-header mode.
    /// </summary>
    [TestFixture]
    public class CsvToJsonTests
    {
        private readonly CsvToolkitActions _sut = new();

        [Test]
        public void CsvToJson_BasicCsv_ReturnsValidJson()
        {
            var csv = "Name,Age,City\nAlice,30,Lisbon\nBob,25,Porto";

            _sut.CsvToJson(csv, ",", true,
                out string jsonResult, out int recordCount, out bool isSuccess, out string errorMessage);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(errorMessage, Is.Empty);
                Assert.That(recordCount, Is.EqualTo(2));
                Assert.That(jsonResult, Is.Not.Empty);
            });

            var parsed = JsonSerializer.Deserialize<JsonElement>(jsonResult);
            Assert.Multiple(() =>
            {
                Assert.That(parsed.ValueKind, Is.EqualTo(JsonValueKind.Array));
                Assert.That(parsed.GetArrayLength(), Is.EqualTo(2));
                Assert.That(parsed[0].GetProperty("Name").GetString(), Is.EqualTo("Alice"));
                Assert.That(parsed[0].GetProperty("Age").GetString(), Is.EqualTo("30"));
                Assert.That(parsed[0].GetProperty("City").GetString(), Is.EqualTo("Lisbon"));
                Assert.That(parsed[1].GetProperty("Name").GetString(), Is.EqualTo("Bob"));
            });
        }

        [Test]
        public void CsvToJson_SemicolonDelimiter_ParsesCorrectly()
        {
            var csv = "Name;Age;City\nAlice;30;Lisbon\nBob;25;Porto";

            _sut.CsvToJson(csv, ";", true,
                out string jsonResult, out int recordCount, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(recordCount, Is.EqualTo(2));
            });

            var parsed = JsonSerializer.Deserialize<JsonElement>(jsonResult);
            Assert.That(parsed[0].GetProperty("Name").GetString(), Is.EqualTo("Alice"));
        }

        [Test]
        public void CsvToJson_TabDelimiter_ParsesCorrectly()
        {
            var csv = "Name\tAge\tCity\nAlice\t30\tLisbon";

            _sut.CsvToJson(csv, "\t", true,
                out string jsonResult, out int recordCount, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(recordCount, Is.EqualTo(1));
            });

            var parsed = JsonSerializer.Deserialize<JsonElement>(jsonResult);
            Assert.That(parsed[0].GetProperty("Name").GetString(), Is.EqualTo("Alice"));
        }

        [Test]
        public void CsvToJson_NoHeader_GeneratesColumnNames()
        {
            var csv = "Alice,30,Lisbon\nBob,25,Porto";

            _sut.CsvToJson(csv, ",", false,
                out string jsonResult, out int recordCount, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(recordCount, Is.EqualTo(2));
            });

            var parsed = JsonSerializer.Deserialize<JsonElement>(jsonResult);
            Assert.Multiple(() =>
            {
                Assert.That(parsed[0].GetProperty("Column1").GetString(), Is.EqualTo("Alice"));
                Assert.That(parsed[0].GetProperty("Column2").GetString(), Is.EqualTo("30"));
                Assert.That(parsed[0].GetProperty("Column3").GetString(), Is.EqualTo("Lisbon"));
            });
        }

        [Test]
        public void CsvToJson_QuotedFieldsWithCommas_ParsesCorrectly()
        {
            var csv = "Name,Description,Value\n\"Smith, John\",\"A \"\"quoted\"\" value\",100";

            _sut.CsvToJson(csv, ",", true,
                out string jsonResult, out int recordCount, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(recordCount, Is.EqualTo(1));
            });

            var parsed = JsonSerializer.Deserialize<JsonElement>(jsonResult);
            Assert.Multiple(() =>
            {
                Assert.That(parsed[0].GetProperty("Name").GetString(), Is.EqualTo("Smith, John"));
                Assert.That(parsed[0].GetProperty("Description").GetString(), Is.EqualTo("A \"quoted\" value"));
                Assert.That(parsed[0].GetProperty("Value").GetString(), Is.EqualTo("100"));
            });
        }

        [Test]
        public void CsvToJson_EmptyContent_ReturnsEmptyArray()
        {
            _sut.CsvToJson("", ",", true,
                out string jsonResult, out int recordCount, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(jsonResult, Is.EqualTo("[]"));
                Assert.That(recordCount, Is.EqualTo(0));
            });
        }

        [Test]
        public void CsvToJson_WhitespaceOnly_ReturnsEmptyArray()
        {
            _sut.CsvToJson("   \n  ", ",", true,
                out string jsonResult, out int recordCount, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(jsonResult, Is.EqualTo("[]"));
                Assert.That(recordCount, Is.EqualTo(0));
            });
        }

        [Test]
        public void CsvToJson_HeaderOnly_ReturnsEmptyArrayWithZeroRecords()
        {
            var csv = "Name,Age,City";

            _sut.CsvToJson(csv, ",", true,
                out string jsonResult, out int recordCount, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(jsonResult, Is.EqualTo("[]"));
                Assert.That(recordCount, Is.EqualTo(0));
            });
        }

        [Test]
        public void CsvToJson_SingleRecord_ReturnsArrayWithOneElement()
        {
            var csv = "Name,Age\nAlice,30";

            _sut.CsvToJson(csv, ",", true,
                out string jsonResult, out int recordCount, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(recordCount, Is.EqualTo(1));
            });

            var parsed = JsonSerializer.Deserialize<JsonElement>(jsonResult);
            Assert.That(parsed.GetArrayLength(), Is.EqualTo(1));
        }

        [Test]
        public void CsvToJson_EmptyDelimiter_DefaultsToComma()
        {
            var csv = "Name,Age\nAlice,30";

            _sut.CsvToJson(csv, "", true,
                out string jsonResult, out int recordCount, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(recordCount, Is.EqualTo(1));
            });

            var parsed = JsonSerializer.Deserialize<JsonElement>(jsonResult);
            Assert.That(parsed[0].GetProperty("Name").GetString(), Is.EqualTo("Alice"));
        }
    }

    /// <summary>
    /// Tests for the JsonToCsv action: JSON to CSV conversion, custom delimiters,
    /// and no-header mode.
    /// </summary>
    [TestFixture]
    public class JsonToCsvTests
    {
        private readonly CsvToolkitActions _sut = new();

        [Test]
        public void JsonToCsv_BasicJson_ReturnsCsv()
        {
            var json = "[{\"Name\":\"Alice\",\"Age\":\"30\"},{\"Name\":\"Bob\",\"Age\":\"25\"}]";

            _sut.JsonToCsv(json, ",", true,
                out string csvResult, out int recordCount, out bool isSuccess, out string errorMessage);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(errorMessage, Is.Empty);
                Assert.That(recordCount, Is.EqualTo(2));
                Assert.That(csvResult, Does.Contain("Name"));
                Assert.That(csvResult, Does.Contain("Alice"));
                Assert.That(csvResult, Does.Contain("Bob"));
            });
        }

        [Test]
        public void JsonToCsv_SemicolonDelimiter_UsesSemicolon()
        {
            var json = "[{\"Name\":\"Alice\",\"Age\":\"30\"}]";

            _sut.JsonToCsv(json, ";", true,
                out string csvResult, out int recordCount, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(recordCount, Is.EqualTo(1));
                Assert.That(csvResult, Does.Contain("Name;Age"));
            });
        }

        [Test]
        public void JsonToCsv_NoHeader_OmitsHeaderRow()
        {
            var json = "[{\"Name\":\"Alice\",\"Age\":\"30\"}]";

            _sut.JsonToCsv(json, ",", false,
                out string csvResult, out int recordCount, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(recordCount, Is.EqualTo(1));
                Assert.That(csvResult, Does.Not.Contain("Name,Age"));
                Assert.That(csvResult, Does.Contain("Alice"));
            });
        }

        [Test]
        public void JsonToCsv_EmptyArray_ReturnsEmptyCsv()
        {
            var json = "[]";

            _sut.JsonToCsv(json, ",", true,
                out string csvResult, out int recordCount, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(csvResult, Is.Empty);
                Assert.That(recordCount, Is.EqualTo(0));
            });
        }

        [Test]
        public void JsonToCsv_EmptyContent_ReturnsEmpty()
        {
            _sut.JsonToCsv("", ",", true,
                out string csvResult, out int recordCount, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(csvResult, Is.Empty);
                Assert.That(recordCount, Is.EqualTo(0));
            });
        }

        [Test]
        public void JsonToCsv_NotAnArray_ReturnsError()
        {
            var json = "{\"Name\":\"Alice\"}";

            _sut.JsonToCsv(json, ",", true,
                out _, out _, out bool isSuccess, out string errorMessage);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.False);
                Assert.That(errorMessage, Does.Contain("array"));
            });
        }

        [Test]
        public void JsonToCsv_FieldsWithCommas_QuotesFieldsProperly()
        {
            var json = "[{\"Name\":\"Smith, John\",\"City\":\"Lisbon\"}]";

            _sut.JsonToCsv(json, ",", true,
                out string csvResult, out int recordCount, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(recordCount, Is.EqualTo(1));
                Assert.That(csvResult, Does.Contain("\"Smith, John\""));
            });
        }

        [Test]
        public void JsonToCsv_NumericValues_WrittenCorrectly()
        {
            var json = "[{\"Name\":\"Alice\",\"Score\":95.5,\"Active\":true}]";

            _sut.JsonToCsv(json, ",", true,
                out string csvResult, out int recordCount, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(recordCount, Is.EqualTo(1));
                Assert.That(csvResult, Does.Contain("95.5"));
                Assert.That(csvResult, Does.Contain("true"));
            });
        }

        [Test]
        public void JsonToCsv_MissingProperty_WritesEmptyField()
        {
            var json = "[{\"Name\":\"Alice\",\"Age\":\"30\"},{\"Name\":\"Bob\"}]";

            _sut.JsonToCsv(json, ",", true,
                out string csvResult, out int recordCount, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(recordCount, Is.EqualTo(2));
            });

            // Bob's row should have an empty Age field
            var lines = csvResult.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(lines, Has.Length.EqualTo(3)); // header + 2 data rows
            Assert.That(lines[2], Does.Contain("Bob"));
        }
    }

    /// <summary>
    /// Tests for the GetCsvHeaders action: header extraction from CSV content.
    /// </summary>
    [TestFixture]
    public class GetCsvHeadersTests
    {
        private readonly CsvToolkitActions _sut = new();

        [Test]
        public void GetCsvHeaders_BasicCsv_ReturnsHeaders()
        {
            var csv = "Name,Age,City\nAlice,30,Lisbon";

            _sut.GetCsvHeaders(csv, ",",
                out List<CsvColumnInfo> headers, out bool isSuccess, out string errorMessage);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(errorMessage, Is.Empty);
                Assert.That(headers, Has.Count.EqualTo(3));
                Assert.That(headers[0].Name, Is.EqualTo("Name"));
                Assert.That(headers[0].Index, Is.EqualTo(0));
                Assert.That(headers[1].Name, Is.EqualTo("Age"));
                Assert.That(headers[1].Index, Is.EqualTo(1));
                Assert.That(headers[2].Name, Is.EqualTo("City"));
                Assert.That(headers[2].Index, Is.EqualTo(2));
            });
        }

        [Test]
        public void GetCsvHeaders_SemicolonDelimiter_ReturnsHeaders()
        {
            var csv = "First;Last;Email\nJohn;Doe;john@example.com";

            _sut.GetCsvHeaders(csv, ";",
                out List<CsvColumnInfo> headers, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(headers, Has.Count.EqualTo(3));
                Assert.That(headers[0].Name, Is.EqualTo("First"));
                Assert.That(headers[1].Name, Is.EqualTo("Last"));
                Assert.That(headers[2].Name, Is.EqualTo("Email"));
            });
        }

        [Test]
        public void GetCsvHeaders_EmptyContent_ReturnsEmptyList()
        {
            _sut.GetCsvHeaders("", ",",
                out List<CsvColumnInfo> headers, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(headers, Is.Empty);
            });
        }

        [Test]
        public void GetCsvHeaders_HeaderOnly_ReturnsHeaders()
        {
            var csv = "Name,Age,City";

            _sut.GetCsvHeaders(csv, ",",
                out List<CsvColumnInfo> headers, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(headers, Has.Count.EqualTo(3));
            });
        }

        [Test]
        public void GetCsvHeaders_SingleColumn_ReturnsSingleHeader()
        {
            var csv = "Name\nAlice\nBob";

            _sut.GetCsvHeaders(csv, ",",
                out List<CsvColumnInfo> headers, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(headers, Has.Count.EqualTo(1));
                Assert.That(headers[0].Name, Is.EqualTo("Name"));
                Assert.That(headers[0].Index, Is.EqualTo(0));
            });
        }
    }

    /// <summary>
    /// Tests for the CountCsvRecords action: record counting with and without headers.
    /// </summary>
    [TestFixture]
    public class CountCsvRecordsTests
    {
        private readonly CsvToolkitActions _sut = new();

        [Test]
        public void CountCsvRecords_WithHeader_CountsDataRowsOnly()
        {
            var csv = "Name,Age\nAlice,30\nBob,25\nCharlie,35";

            _sut.CountCsvRecords(csv, ",", true,
                out int recordCount, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(recordCount, Is.EqualTo(3));
            });
        }

        [Test]
        public void CountCsvRecords_WithoutHeader_CountsAllRows()
        {
            var csv = "Alice,30\nBob,25\nCharlie,35";

            _sut.CountCsvRecords(csv, ",", false,
                out int recordCount, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(recordCount, Is.EqualTo(3));
            });
        }

        [Test]
        public void CountCsvRecords_HeaderOnly_ReturnsZero()
        {
            var csv = "Name,Age,City";

            _sut.CountCsvRecords(csv, ",", true,
                out int recordCount, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(recordCount, Is.EqualTo(0));
            });
        }

        [Test]
        public void CountCsvRecords_EmptyContent_ReturnsZero()
        {
            _sut.CountCsvRecords("", ",", true,
                out int recordCount, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(recordCount, Is.EqualTo(0));
            });
        }

        [Test]
        public void CountCsvRecords_SingleRow_ReturnsOne()
        {
            var csv = "Name,Age\nAlice,30";

            _sut.CountCsvRecords(csv, ",", true,
                out int recordCount, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(recordCount, Is.EqualTo(1));
            });
        }

        [Test]
        public void CountCsvRecords_SemicolonDelimiter_CountsCorrectly()
        {
            var csv = "Name;Age\nAlice;30\nBob;25";

            _sut.CountCsvRecords(csv, ";", true,
                out int recordCount, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(recordCount, Is.EqualTo(2));
            });
        }
    }

    /// <summary>
    /// Tests for the ValidateCsv action: validation with valid and invalid CSV content.
    /// </summary>
    [TestFixture]
    public class ValidateCsvTests
    {
        private readonly CsvToolkitActions _sut = new();

        [Test]
        public void ValidateCsv_ValidCsv_ReturnsValid()
        {
            var csv = "Name,Age,City\nAlice,30,Lisbon\nBob,25,Porto";

            _sut.ValidateCsv(csv, ",", true,
                out CsvValidationResult result, out bool isSuccess, out string errorMessage);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(errorMessage, Is.Empty);
                Assert.That(result.IsValid, Is.True);
                Assert.That(result.RecordCount, Is.EqualTo(2));
                Assert.That(result.ColumnCount, Is.EqualTo(3));
                Assert.That(result.ErrorDetails, Is.Empty);
            });
        }

        [Test]
        public void ValidateCsv_InconsistentColumns_ReportsIssues()
        {
            var csv = "Name,Age,City\nAlice,30\nBob,25,Porto,Extra";

            _sut.ValidateCsv(csv, ",", true,
                out CsvValidationResult result, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(result.IsValid, Is.False);
                Assert.That(result.RecordCount, Is.EqualTo(2));
                Assert.That(result.ColumnCount, Is.EqualTo(3));
                Assert.That(result.ErrorDetails, Is.Not.Empty);
            });
        }

        [Test]
        public void ValidateCsv_EmptyContent_ReturnsValidWithZeroCounts()
        {
            _sut.ValidateCsv("", ",", true,
                out CsvValidationResult result, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(result.IsValid, Is.True);
                Assert.That(result.RecordCount, Is.EqualTo(0));
                Assert.That(result.ColumnCount, Is.EqualTo(0));
            });
        }

        [Test]
        public void ValidateCsv_NoHeader_ValidatesRowConsistency()
        {
            var csv = "Alice,30,Lisbon\nBob,25,Porto";

            _sut.ValidateCsv(csv, ",", false,
                out CsvValidationResult result, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(result.IsValid, Is.True);
                Assert.That(result.RecordCount, Is.EqualTo(2));
                Assert.That(result.ColumnCount, Is.EqualTo(3));
            });
        }

        [Test]
        public void ValidateCsv_NoHeader_InconsistentColumns_ReportsIssues()
        {
            var csv = "Alice,30,Lisbon\nBob,25";

            _sut.ValidateCsv(csv, ",", false,
                out CsvValidationResult result, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(result.IsValid, Is.False);
                Assert.That(result.ErrorDetails, Does.Contain("Row 2"));
            });
        }

        [Test]
        public void ValidateCsv_SingleRowWithHeader_ReturnsZeroRecords()
        {
            var csv = "Name,Age,City";

            _sut.ValidateCsv(csv, ",", true,
                out CsvValidationResult result, out bool isSuccess, out _);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.True);
                Assert.That(result.IsValid, Is.True);
                Assert.That(result.RecordCount, Is.EqualTo(0));
                Assert.That(result.ColumnCount, Is.EqualTo(3));
            });
        }
    }

    /// <summary>
    /// Tests for the CsvColumnInfo and CsvValidationResult structures:
    /// default constructors, field assignment, and value type verification.
    /// </summary>
    [TestFixture]
    public class CsvStructureTests
    {
        [Test]
        public void CsvColumnInfo_DefaultConstructor_InitializesToDefaults()
        {
            var column = new CsvColumnInfo();

            Assert.Multiple(() =>
            {
                Assert.That(column.Name, Is.EqualTo(string.Empty));
                Assert.That(column.Index, Is.EqualTo(0));
            });
        }

        [Test]
        public void CsvColumnInfo_FieldAssignment_RetainsValues()
        {
            var column = new CsvColumnInfo
            {
                Name = "TestColumn",
                Index = 5
            };

            Assert.Multiple(() =>
            {
                Assert.That(column.Name, Is.EqualTo("TestColumn"));
                Assert.That(column.Index, Is.EqualTo(5));
            });
        }

        [Test]
        public void CsvColumnInfo_IsValueType()
        {
            Assert.That(typeof(CsvColumnInfo).IsValueType, Is.True,
                "CsvColumnInfo must be a struct (value type) for ODC compatibility");
        }

        [Test]
        public void CsvValidationResult_DefaultConstructor_InitializesToDefaults()
        {
            var result = new CsvValidationResult();

            Assert.Multiple(() =>
            {
                Assert.That(result.IsValid, Is.False);
                Assert.That(result.RecordCount, Is.EqualTo(0));
                Assert.That(result.ColumnCount, Is.EqualTo(0));
                Assert.That(result.ErrorDetails, Is.EqualTo(string.Empty));
            });
        }

        [Test]
        public void CsvValidationResult_FieldAssignment_RetainsValues()
        {
            var result = new CsvValidationResult
            {
                IsValid = true,
                RecordCount = 42,
                ColumnCount = 5,
                ErrorDetails = "Some issues"
            };

            Assert.Multiple(() =>
            {
                Assert.That(result.IsValid, Is.True);
                Assert.That(result.RecordCount, Is.EqualTo(42));
                Assert.That(result.ColumnCount, Is.EqualTo(5));
                Assert.That(result.ErrorDetails, Is.EqualTo("Some issues"));
            });
        }

        [Test]
        public void CsvValidationResult_IsValueType()
        {
            Assert.That(typeof(CsvValidationResult).IsValueType, Is.True,
                "CsvValidationResult must be a struct (value type) for ODC compatibility");
        }
    }

    /// <summary>
    /// Tests for error handling across all actions. These validate that the wrapper
    /// correctly catches exceptions and returns structured error responses.
    /// </summary>
    [TestFixture]
    public class ErrorHandlingTests
    {
        private readonly CsvToolkitActions _sut = new();

        [Test]
        public void CsvToJson_NullContent_ReturnsError()
        {
            _sut.CsvToJson(null!, ",", true,
                out string jsonResult, out int recordCount, out bool isSuccess, out string errorMessage);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.False);
                Assert.That(errorMessage, Is.Not.Empty);
                Assert.That(errorMessage, Does.Contain("null"));
                Assert.That(jsonResult, Is.EqualTo(string.Empty));
                Assert.That(recordCount, Is.EqualTo(0));
            });
        }

        [Test]
        public void JsonToCsv_NullContent_ReturnsError()
        {
            _sut.JsonToCsv(null!, ",", true,
                out string csvResult, out int recordCount, out bool isSuccess, out string errorMessage);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.False);
                Assert.That(errorMessage, Is.Not.Empty);
                Assert.That(errorMessage, Does.Contain("null"));
                Assert.That(csvResult, Is.EqualTo(string.Empty));
                Assert.That(recordCount, Is.EqualTo(0));
            });
        }

        [Test]
        public void JsonToCsv_InvalidJson_ReturnsError()
        {
            _sut.JsonToCsv("{not valid json", ",", true,
                out _, out _, out bool isSuccess, out string errorMessage);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.False);
                Assert.That(errorMessage, Is.Not.Empty);
            });
        }

        [Test]
        public void GetCsvHeaders_NullContent_ReturnsError()
        {
            _sut.GetCsvHeaders(null!, ",",
                out List<CsvColumnInfo> headers, out bool isSuccess, out string errorMessage);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.False);
                Assert.That(errorMessage, Is.Not.Empty);
                Assert.That(errorMessage, Does.Contain("null"));
                Assert.That(headers, Is.Not.Null);
                Assert.That(headers, Is.Empty);
            });
        }

        [Test]
        public void CountCsvRecords_NullContent_ReturnsError()
        {
            _sut.CountCsvRecords(null!, ",", true,
                out int recordCount, out bool isSuccess, out string errorMessage);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.False);
                Assert.That(errorMessage, Is.Not.Empty);
                Assert.That(errorMessage, Does.Contain("null"));
                Assert.That(recordCount, Is.EqualTo(0));
            });
        }

        [Test]
        public void ValidateCsv_NullContent_ReturnsError()
        {
            _sut.ValidateCsv(null!, ",", true,
                out CsvValidationResult result, out bool isSuccess, out string errorMessage);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.False);
                Assert.That(errorMessage, Is.Not.Empty);
                Assert.That(errorMessage, Does.Contain("null"));
                Assert.That(result.IsValid, Is.False);
                Assert.That(result.RecordCount, Is.EqualTo(0));
            });
        }

        [Test]
        public void CsvToJson_OnError_AllOutParamsHaveSafeDefaults()
        {
            _sut.CsvToJson(null!, ",", true,
                out string jsonResult, out int recordCount, out bool isSuccess, out string errorMessage);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.False);
                Assert.That(jsonResult, Is.EqualTo(string.Empty));
                Assert.That(recordCount, Is.EqualTo(0));
                Assert.That(errorMessage, Is.Not.Empty);
            });
        }

        [Test]
        public void JsonToCsv_OnError_AllOutParamsHaveSafeDefaults()
        {
            _sut.JsonToCsv(null!, ",", true,
                out string csvResult, out int recordCount, out bool isSuccess, out string errorMessage);

            Assert.Multiple(() =>
            {
                Assert.That(isSuccess, Is.False);
                Assert.That(csvResult, Is.EqualTo(string.Empty));
                Assert.That(recordCount, Is.EqualTo(0));
                Assert.That(errorMessage, Is.Not.Empty);
            });
        }
    }

    /// <summary>
    /// Reflection tests verifying OSInterface, OSAction, OSStructure, OSStructureField
    /// attributes, void returns, and interface compliance.
    /// </summary>
    [TestFixture]
    public class InterfaceComplianceTests
    {
        [Test]
        public void CsvToolkitActions_ImplementsICsvToolkit()
        {
            Assert.That(typeof(CsvToolkitActions).GetInterfaces(),
                Does.Contain(typeof(ICsvToolkit)));
        }

        [Test]
        public void ICsvToolkit_HasOSInterfaceAttribute()
        {
            var attr = typeof(ICsvToolkit).GetCustomAttributes(
                typeof(OutSystems.ExternalLibraries.SDK.OSInterfaceAttribute), false);

            Assert.That(attr, Has.Length.EqualTo(1));
        }

        [Test]
        public void ICsvToolkit_HasFiveMethods()
        {
            var methods = typeof(ICsvToolkit).GetMethods();

            Assert.That(methods, Has.Length.EqualTo(5));
        }

        [Test]
        public void ICsvToolkit_AllMethodsReturnVoid()
        {
            var methods = typeof(ICsvToolkit).GetMethods();

            foreach (var method in methods)
            {
                Assert.That(method.ReturnType, Is.EqualTo(typeof(void)),
                    $"Method {method.Name} should return void");
            }
        }

        [Test]
        public void ICsvToolkit_AllMethodsHaveOSActionAttribute()
        {
            var methods = typeof(ICsvToolkit).GetMethods();

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttributes(
                    typeof(OutSystems.ExternalLibraries.SDK.OSActionAttribute), false);
                Assert.That(attr, Has.Length.EqualTo(1),
                    $"Method {method.Name} should have [OSAction] attribute");
            }
        }

        [Test]
        public void ICsvToolkit_AllMethodsHaveIsSuccessAndErrorMessageParams()
        {
            var methods = typeof(ICsvToolkit).GetMethods();

            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                var lastTwo = parameters.Skip(parameters.Length - 2).ToArray();

                Assert.Multiple(() =>
                {
                    Assert.That(lastTwo[0].Name, Is.EqualTo("isSuccess"),
                        $"Method {method.Name}: second-to-last param should be 'isSuccess'");
                    Assert.That(lastTwo[0].ParameterType, Is.EqualTo(typeof(bool).MakeByRefType()),
                        $"Method {method.Name}: isSuccess should be out bool");
                    Assert.That(lastTwo[1].Name, Is.EqualTo("errorMessage"),
                        $"Method {method.Name}: last param should be 'errorMessage'");
                    Assert.That(lastTwo[1].ParameterType, Is.EqualTo(typeof(string).MakeByRefType()),
                        $"Method {method.Name}: errorMessage should be out string");
                });
            }
        }

        [Test]
        public void CsvColumnInfo_HasOSStructureAttribute()
        {
            var attr = typeof(CsvColumnInfo).GetCustomAttributes(
                typeof(OutSystems.ExternalLibraries.SDK.OSStructureAttribute), false);

            Assert.That(attr, Has.Length.EqualTo(1));
        }

        [Test]
        public void CsvColumnInfo_AllPublicFieldsHaveOSStructureFieldAttribute()
        {
            var fields = typeof(CsvColumnInfo).GetFields(
                BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                var attr = field.GetCustomAttributes(
                    typeof(OutSystems.ExternalLibraries.SDK.OSStructureFieldAttribute), false);
                Assert.That(attr, Has.Length.EqualTo(1),
                    $"Field {field.Name} should have [OSStructureField] attribute");
            }
        }

        [Test]
        public void CsvValidationResult_HasOSStructureAttribute()
        {
            var attr = typeof(CsvValidationResult).GetCustomAttributes(
                typeof(OutSystems.ExternalLibraries.SDK.OSStructureAttribute), false);

            Assert.That(attr, Has.Length.EqualTo(1));
        }

        [Test]
        public void CsvValidationResult_AllPublicFieldsHaveOSStructureFieldAttribute()
        {
            var fields = typeof(CsvValidationResult).GetFields(
                BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                var attr = field.GetCustomAttributes(
                    typeof(OutSystems.ExternalLibraries.SDK.OSStructureFieldAttribute), false);
                Assert.That(attr, Has.Length.EqualTo(1),
                    $"Field {field.Name} should have [OSStructureField] attribute");
            }
        }

        [Test]
        public void ICsvToolkit_AllParametersHaveOSParameterAttribute()
        {
            var methods = typeof(ICsvToolkit).GetMethods();

            foreach (var method in methods)
            {
                foreach (var param in method.GetParameters())
                {
                    var attr = param.GetCustomAttributes(
                        typeof(OutSystems.ExternalLibraries.SDK.OSParameterAttribute), false);
                    Assert.That(attr, Has.Length.EqualTo(1),
                        $"Parameter {param.Name} on method {method.Name} should have [OSParameter]");
                }
            }
        }

        [Test]
        public void CsvColumnInfo_HasNoProperties_OnlyFields()
        {
            var properties = typeof(CsvColumnInfo).GetProperties(
                BindingFlags.Public | BindingFlags.Instance);

            Assert.That(properties, Is.Empty,
                "CsvColumnInfo should use public fields, not properties, for ODC compatibility");
        }

        [Test]
        public void CsvValidationResult_HasNoProperties_OnlyFields()
        {
            var properties = typeof(CsvValidationResult).GetProperties(
                BindingFlags.Public | BindingFlags.Instance);

            Assert.That(properties, Is.Empty,
                "CsvValidationResult should use public fields, not properties, for ODC compatibility");
        }
    }
}
