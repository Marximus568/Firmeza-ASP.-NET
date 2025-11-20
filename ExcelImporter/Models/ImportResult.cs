namespace ExcelImporter;

public class ImportResult
{
    public int TotalRows { get; set; }
    public int Inserted { get; set; }
    public int Updated { get; set; }
    public int Errors { get; set; }
    public List<ImportError> ErrorList { get; set; } = new();
    
    public bool Success => Errors == 0;
    public string Message => Success 
        ? $"Import completed: {Inserted} inserted, {Updated} updated" 
        : $"Import failed: {Errors} errors found";
}

public class ImportError
{
    public int RowNumber { get; set; }
    public string Field { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}