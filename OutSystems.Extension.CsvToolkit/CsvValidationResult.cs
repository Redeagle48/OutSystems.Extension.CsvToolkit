using OutSystems.ExternalLibraries.SDK;

namespace OutSystems.Extension.CsvToolkit
{
    /// <summary>
    /// Represents the result of validating CSV content structure, including record and column
    /// counts and any structural issues detected.
    /// </summary>
    [OSStructure(Description = "Represents the result of validating CSV content structure, including record and column counts and any structural issues detected.")]
    public struct CsvValidationResult
    {
        /// <summary>Whether the CSV is structurally valid.</summary>
        [OSStructureField(Description = "Whether the CSV is structurally valid.")]
        public bool IsValid;

        /// <summary>Total number of data records.</summary>
        [OSStructureField(Description = "Total number of data records.")]
        public int RecordCount;

        /// <summary>Number of columns detected.</summary>
        [OSStructureField(Description = "Number of columns detected.")]
        public int ColumnCount;

        /// <summary>Description of any structural issues found (empty if valid).</summary>
        [OSStructureField(Description = "Description of any structural issues found (empty if valid).")]
        public string ErrorDetails;

        /// <summary>
        /// Initializes a new CsvValidationResult with default empty values.
        /// </summary>
        public CsvValidationResult()
        {
            IsValid = false;
            RecordCount = 0;
            ColumnCount = 0;
            ErrorDetails = string.Empty;
        }
    }
}
