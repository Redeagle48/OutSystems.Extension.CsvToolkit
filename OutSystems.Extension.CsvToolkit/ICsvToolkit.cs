using OutSystems.ExternalLibraries.SDK;

namespace OutSystems.Extension.CsvToolkit
{
    /// <summary>
    /// Provides actions for parsing, converting, validating, and inspecting CSV data.
    /// </summary>
    [OSInterface(
        Name = "CsvToolkit",
        Description = "Provides actions for parsing, converting, validating, and inspecting CSV data.",
        IconResourceName = "OutSystems.Extension.CsvToolkit.resources.csv_lib_app_icon.png"
    )]
    public interface ICsvToolkit
    {
        /// <summary>
        /// Parses CSV text content and converts it to a JSON array string.
        /// OutSystems can then use JSON Deserialize to convert it to a Record List.
        /// </summary>
        /// <param name="csvContent">The CSV text content to parse.</param>
        /// <param name="delimiter">The field delimiter (defaults to comma if empty).</param>
        /// <param name="hasHeaderRow">True if the first row contains column headers; false to auto-generate Column1, Column2, etc.</param>
        /// <param name="jsonResult">The resulting JSON array string from the conversion.</param>
        /// <param name="recordCount">The number of data records parsed.</param>
        /// <param name="isSuccess">True if the operation completed successfully.</param>
        /// <param name="errorMessage">Error message if the operation failed; empty on success.</param>
        [OSAction(
            Description = "Parses CSV text content and converts it to a JSON array string. OutSystems can then use JSON Deserialize to convert it to a Record List.",
            IconResourceName = "OutSystems.Extension.CsvToolkit.resources.csv_lib_app_icon.png"
        )]
        void CsvToJson(
            [OSParameter(Description = "The CSV text content to parse.")]
            string csvContent,

            [OSParameter(Description = "The field delimiter (defaults to comma if empty).")]
            string delimiter,

            [OSParameter(Description = "True if the first row contains column headers; false to auto-generate Column1, Column2, etc.")]
            bool hasHeaderRow,

            [OSParameter(Description = "The resulting JSON array string from the conversion.")]
            out string jsonResult,

            [OSParameter(Description = "The number of data records parsed.")]
            out int recordCount,

            [OSParameter(Description = "True if the operation completed successfully.")]
            out bool isSuccess,

            [OSParameter(Description = "Error message if the operation failed; empty on success.")]
            out string errorMessage
        );

        /// <summary>
        /// Converts a JSON array string to CSV text format for export or download.
        /// Use JSON Serialize on a Record List to generate the input for this action.
        /// </summary>
        /// <param name="jsonContent">The JSON array string to convert. Use JSON Serialize on a Record List to generate this input.</param>
        /// <param name="delimiter">The field delimiter (defaults to comma if empty).</param>
        /// <param name="includeHeaderRow">True to include a header row with column names.</param>
        /// <param name="csvResult">The resulting CSV text from the conversion.</param>
        /// <param name="recordCount">The number of data records written.</param>
        /// <param name="isSuccess">True if the operation completed successfully.</param>
        /// <param name="errorMessage">Error message if the operation failed; empty on success.</param>
        [OSAction(
            Description = "Converts a JSON array string to CSV text format for export or download. Use JSON Serialize on a Record List to generate the input.",
            IconResourceName = "OutSystems.Extension.CsvToolkit.resources.csv_lib_app_icon.png"
        )]
        void JsonToCsv(
            [OSParameter(Description = "The JSON array string to convert. Use JSON Serialize on a Record List to generate this input.")]
            string jsonContent,

            [OSParameter(Description = "The field delimiter (defaults to comma if empty).")]
            string delimiter,

            [OSParameter(Description = "True to include a header row with column names.")]
            bool includeHeaderRow,

            [OSParameter(Description = "The resulting CSV text from the conversion.")]
            out string csvResult,

            [OSParameter(Description = "The number of data records written.")]
            out int recordCount,

            [OSParameter(Description = "True if the operation completed successfully.")]
            out bool isSuccess,

            [OSParameter(Description = "Error message if the operation failed; empty on success.")]
            out string errorMessage
        );

        /// <summary>
        /// Extracts column header names from CSV content.
        /// Use this to dynamically inspect CSV structure before conversion.
        /// </summary>
        /// <param name="csvContent">The CSV text content to extract headers from.</param>
        /// <param name="delimiter">The field delimiter (defaults to comma if empty).</param>
        /// <param name="headers">The list of column headers with their names and positions.</param>
        /// <param name="isSuccess">True if the operation completed successfully.</param>
        /// <param name="errorMessage">Error message if the operation failed; empty on success.</param>
        [OSAction(
            Description = "Extracts column header names from CSV content. Use this to dynamically inspect CSV structure before conversion.",
            IconResourceName = "OutSystems.Extension.CsvToolkit.resources.csv_lib_app_icon.png"
        )]
        void GetCsvHeaders(
            [OSParameter(Description = "The CSV text content to extract headers from.")]
            string csvContent,

            [OSParameter(Description = "The field delimiter (defaults to comma if empty).")]
            string delimiter,

            [OSParameter(Description = "The list of column headers with their names and positions.")]
            out List<CsvColumnInfo> headers,

            [OSParameter(Description = "True if the operation completed successfully.")]
            out bool isSuccess,

            [OSParameter(Description = "Error message if the operation failed; empty on success.")]
            out string errorMessage
        );

        /// <summary>
        /// Counts the number of data records in CSV content (excludes the header row if present).
        /// </summary>
        /// <param name="csvContent">The CSV text content to count records in.</param>
        /// <param name="delimiter">The field delimiter (defaults to comma if empty).</param>
        /// <param name="hasHeaderRow">True if the first row contains column headers.</param>
        /// <param name="recordCount">The number of data records found.</param>
        /// <param name="isSuccess">True if the operation completed successfully.</param>
        /// <param name="errorMessage">Error message if the operation failed; empty on success.</param>
        [OSAction(
            Description = "Counts the number of data records in CSV content (excludes the header row if present).",
            IconResourceName = "OutSystems.Extension.CsvToolkit.resources.csv_lib_app_icon.png"
        )]
        void CountCsvRecords(
            [OSParameter(Description = "The CSV text content to count records in.")]
            string csvContent,

            [OSParameter(Description = "The field delimiter (defaults to comma if empty).")]
            string delimiter,

            [OSParameter(Description = "True if the first row contains column headers.")]
            bool hasHeaderRow,

            [OSParameter(Description = "The number of data records found.")]
            out int recordCount,

            [OSParameter(Description = "True if the operation completed successfully.")]
            out bool isSuccess,

            [OSParameter(Description = "Error message if the operation failed; empty on success.")]
            out string errorMessage
        );

        /// <summary>
        /// Validates CSV structure for consistent column counts across all rows and returns any issues found.
        /// </summary>
        /// <param name="csvContent">The CSV text content to validate.</param>
        /// <param name="delimiter">The field delimiter (defaults to comma if empty).</param>
        /// <param name="hasHeaderRow">True if the first row contains column headers.</param>
        /// <param name="validationResult">The validation result with structural diagnostics.</param>
        /// <param name="isSuccess">True if the validation ran without errors. Check IsValid in the result for CSV validity.</param>
        /// <param name="errorMessage">Error message if the operation failed; empty on success.</param>
        [OSAction(
            Description = "Validates CSV structure for consistent column counts across all rows and returns any issues found.",
            IconResourceName = "OutSystems.Extension.CsvToolkit.resources.csv_lib_app_icon.png"
        )]
        void ValidateCsv(
            [OSParameter(Description = "The CSV text content to validate.")]
            string csvContent,

            [OSParameter(Description = "The field delimiter (defaults to comma if empty).")]
            string delimiter,

            [OSParameter(Description = "True if the first row contains column headers.")]
            bool hasHeaderRow,

            [OSParameter(Description = "The validation result with structural diagnostics.")]
            out CsvValidationResult validationResult,

            [OSParameter(Description = "True if the validation ran without errors. Check IsValid in the result for CSV validity.")]
            out bool isSuccess,

            [OSParameter(Description = "Error message if the operation failed; empty on success.")]
            out string errorMessage
        );
    }
}
