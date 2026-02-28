using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;

namespace OutSystems.Extension.CsvToolkit
{
    /// <summary>
    /// Implementation of the ICsvToolkit interface providing CSV parsing, conversion,
    /// validation, and inspection actions.
    /// </summary>
    public class CsvToolkitActions : ICsvToolkit
    {
        /// <summary>
        /// Creates a CsvConfiguration with the specified delimiter and header behavior.
        /// </summary>
        /// <param name="delimiter">The delimiter string. If null or empty, defaults to comma.</param>
        /// <param name="hasHeaderRow">Whether the CSV has a header row.</param>
        /// <returns>A configured CsvConfiguration instance.</returns>
        private static CsvConfiguration CreateConfig(string delimiter, bool hasHeaderRow)
        {
            var effectiveDelimiter = string.IsNullOrEmpty(delimiter) ? "," : delimiter;

            return new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = effectiveDelimiter,
                HasHeaderRecord = hasHeaderRow,
                MissingFieldFound = null,
                BadDataFound = null
            };
        }

        /// <inheritdoc />
        public void CsvToJson(
            string csvContent, string delimiter, bool hasHeaderRow,
            out string jsonResult, out int recordCount, out bool isSuccess, out string errorMessage)
        {
            jsonResult = string.Empty;
            recordCount = 0;
            isSuccess = false;
            errorMessage = string.Empty;

            try
            {
                ArgumentNullException.ThrowIfNull(csvContent);

                if (string.IsNullOrWhiteSpace(csvContent))
                {
                    jsonResult = "[]";
                    recordCount = 0;
                    isSuccess = true;
                    return;
                }

                var config = CreateConfig(delimiter, hasHeaderRow);
                var records = new List<Dictionary<string, string>>();

                using (var reader = new StringReader(csvContent))
                using (var csv = new CsvReader(reader, config))
                {
                    if (hasHeaderRow)
                    {
                        csv.Read();
                        csv.ReadHeader();
                        var headerRecord = csv.HeaderRecord ?? Array.Empty<string>();

                        while (csv.Read())
                        {
                            var record = new Dictionary<string, string>();
                            for (int i = 0; i < headerRecord.Length; i++)
                            {
                                var fieldValue = csv.TryGetField<string>(i, out var value) ? value ?? string.Empty : string.Empty;
                                record[headerRecord[i]] = fieldValue;
                            }
                            records.Add(record);
                        }
                    }
                    else
                    {
                        // No header; determine column count from first row
                        int columnCount = 0;
                        while (csv.Read())
                        {
                            if (columnCount == 0)
                            {
                                columnCount = csv.Parser.Count;
                            }

                            var record = new Dictionary<string, string>();
                            for (int i = 0; i < columnCount; i++)
                            {
                                var fieldValue = csv.TryGetField<string>(i, out var value) ? value ?? string.Empty : string.Empty;
                                record[$"Column{i + 1}"] = fieldValue;
                            }
                            records.Add(record);
                        }
                    }
                }

                jsonResult = JsonSerializer.Serialize(records, new JsonSerializerOptions
                {
                    WriteIndented = false
                });
                recordCount = records.Count;
                isSuccess = true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        /// <inheritdoc />
        public void JsonToCsv(
            string jsonContent, string delimiter, bool includeHeaderRow,
            out string csvResult, out int recordCount, out bool isSuccess, out string errorMessage)
        {
            csvResult = string.Empty;
            recordCount = 0;
            isSuccess = false;
            errorMessage = string.Empty;

            try
            {
                ArgumentNullException.ThrowIfNull(jsonContent);

                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    csvResult = string.Empty;
                    recordCount = 0;
                    isSuccess = true;
                    return;
                }

                var jsonArray = JsonSerializer.Deserialize<JsonElement>(jsonContent);

                if (jsonArray.ValueKind != JsonValueKind.Array)
                {
                    throw new ArgumentException("JSON content must be an array.", nameof(jsonContent));
                }

                var items = jsonArray.EnumerateArray().ToList();

                if (items.Count == 0)
                {
                    csvResult = string.Empty;
                    recordCount = 0;
                    isSuccess = true;
                    return;
                }

                // Extract keys from the first object to use as headers
                var headers = new List<string>();
                if (items[0].ValueKind == JsonValueKind.Object)
                {
                    foreach (var prop in items[0].EnumerateObject())
                    {
                        headers.Add(prop.Name);
                    }
                }
                else
                {
                    throw new ArgumentException("JSON array elements must be objects.", nameof(jsonContent));
                }

                var effectiveDelimiter = string.IsNullOrEmpty(delimiter) ? "," : delimiter;

                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = effectiveDelimiter,
                    HasHeaderRecord = false // We'll write headers manually for control
                };

                using var writer = new StringWriter();
                using (var csv = new CsvWriter(writer, config))
                {
                    // Write header row if requested
                    if (includeHeaderRow)
                    {
                        foreach (var header in headers)
                        {
                            csv.WriteField(header);
                        }
                        csv.NextRecord();
                    }

                    // Write data rows
                    int count = 0;
                    foreach (var item in items)
                    {
                        if (item.ValueKind != JsonValueKind.Object) continue;

                        foreach (var header in headers)
                        {
                            if (item.TryGetProperty(header, out var value))
                            {
                                csv.WriteField(GetJsonValueAsString(value));
                            }
                            else
                            {
                                csv.WriteField(string.Empty);
                            }
                        }
                        csv.NextRecord();
                        count++;
                    }
                    recordCount = count;
                }

                csvResult = writer.ToString();
                isSuccess = true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        /// <inheritdoc />
        public void GetCsvHeaders(
            string csvContent, string delimiter,
            out List<CsvColumnInfo> headers, out bool isSuccess, out string errorMessage)
        {
            headers = new List<CsvColumnInfo>();
            isSuccess = false;
            errorMessage = string.Empty;

            try
            {
                ArgumentNullException.ThrowIfNull(csvContent);

                if (string.IsNullOrWhiteSpace(csvContent))
                {
                    isSuccess = true;
                    return;
                }

                var config = CreateConfig(delimiter, true);

                using var reader = new StringReader(csvContent);
                using var csv = new CsvReader(reader, config);

                csv.Read();
                csv.ReadHeader();
                var headerRecord = csv.HeaderRecord ?? Array.Empty<string>();

                for (int i = 0; i < headerRecord.Length; i++)
                {
                    headers.Add(new CsvColumnInfo
                    {
                        Name = headerRecord[i],
                        Index = i
                    });
                }

                isSuccess = true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        /// <inheritdoc />
        public void CountCsvRecords(
            string csvContent, string delimiter, bool hasHeaderRow,
            out int recordCount, out bool isSuccess, out string errorMessage)
        {
            recordCount = 0;
            isSuccess = false;
            errorMessage = string.Empty;

            try
            {
                ArgumentNullException.ThrowIfNull(csvContent);

                if (string.IsNullOrWhiteSpace(csvContent))
                {
                    recordCount = 0;
                    isSuccess = true;
                    return;
                }

                var config = CreateConfig(delimiter, hasHeaderRow);
                int count = 0;

                using (var reader = new StringReader(csvContent))
                using (var csv = new CsvReader(reader, config))
                {
                    if (hasHeaderRow)
                    {
                        csv.Read();
                        csv.ReadHeader();
                    }

                    while (csv.Read())
                    {
                        count++;
                    }
                }

                recordCount = count;
                isSuccess = true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        /// <inheritdoc />
        public void ValidateCsv(
            string csvContent, string delimiter, bool hasHeaderRow,
            out CsvValidationResult validationResult, out bool isSuccess, out string errorMessage)
        {
            validationResult = new CsvValidationResult();
            isSuccess = false;
            errorMessage = string.Empty;

            try
            {
                ArgumentNullException.ThrowIfNull(csvContent);

                if (string.IsNullOrWhiteSpace(csvContent))
                {
                    validationResult = new CsvValidationResult
                    {
                        IsValid = true,
                        RecordCount = 0,
                        ColumnCount = 0,
                        ErrorDetails = string.Empty
                    };
                    isSuccess = true;
                    return;
                }

                var effectiveDelimiter = string.IsNullOrEmpty(delimiter) ? "," : delimiter;

                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = effectiveDelimiter,
                    HasHeaderRecord = hasHeaderRow,
                    MissingFieldFound = null,
                    BadDataFound = null
                };

                int expectedColumnCount = 0;
                int recordCount = 0;
                var issues = new List<string>();

                using (var reader = new StringReader(csvContent))
                using (var csv = new CsvReader(reader, config))
                {
                    if (hasHeaderRow)
                    {
                        csv.Read();
                        csv.ReadHeader();
                        expectedColumnCount = csv.HeaderRecord?.Length ?? 0;
                    }

                    while (csv.Read())
                    {
                        recordCount++;
                        int currentFieldCount = csv.Parser.Count;

                        if (expectedColumnCount == 0)
                        {
                            expectedColumnCount = currentFieldCount;
                        }
                        else if (currentFieldCount != expectedColumnCount)
                        {
                            issues.Add($"Row {recordCount} has {currentFieldCount} fields (expected {expectedColumnCount}).");
                        }
                    }
                }

                bool valid = issues.Count == 0;
                validationResult = new CsvValidationResult
                {
                    IsValid = valid,
                    RecordCount = recordCount,
                    ColumnCount = expectedColumnCount,
                    ErrorDetails = valid ? string.Empty : string.Join(" ", issues)
                };
                isSuccess = true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        /// <summary>
        /// Converts a JsonElement value to its string representation.
        /// </summary>
        /// <param name="element">The JSON element to convert.</param>
        /// <returns>The string representation of the value.</returns>
        private static string GetJsonValueAsString(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString() ?? string.Empty,
                JsonValueKind.Number => element.GetRawText(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                JsonValueKind.Null => string.Empty,
                _ => element.GetRawText()
            };
        }
    }
}
