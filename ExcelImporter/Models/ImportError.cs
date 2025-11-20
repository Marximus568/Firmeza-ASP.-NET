namespace ExcelImporter.Models
{
    /// <summary>
    /// Represents a validation or processing error for a specific Excel row.
    /// </summary>
    public class ImportError
    {
        public int RowNumber { get; set; }
        public string Field { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}