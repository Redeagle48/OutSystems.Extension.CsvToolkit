using OutSystems.ExternalLibraries.SDK;

namespace OutSystems.Extension.CsvToolkit
{
    /// <summary>
    /// Represents a column from CSV content, including its header name and position index.
    /// </summary>
    [OSStructure(Description = "Represents a column from CSV content, including its header name and position index.")]
    public struct CsvColumnInfo
    {
        /// <summary>The column header name.</summary>
        [OSStructureField(Description = "The column header name.")]
        public string Name;

        /// <summary>The column position (starts at 0 for the first column).</summary>
        [OSStructureField(Description = "The column position (starts at 0 for the first column).")]
        public int Index;

        /// <summary>
        /// Initializes a new CsvColumnInfo with default empty values.
        /// </summary>
        public CsvColumnInfo()
        {
            Name = string.Empty;
            Index = 0;
        }
    }
}
