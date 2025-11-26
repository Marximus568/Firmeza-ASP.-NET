namespace ExcelImporter.Models;

/// <summary>
/// Enum representing the type of entity detected in an Excel row
/// </summary>
public enum EntityType
{
    Unknown,
    Client,
    Product,
    Sale,
    SaleItem
}

/// <summary>
/// Represents a row of data classified by entity type
/// </summary>
public class ClassifiedRow
{
    public int RowNumber { get; set; }
    public EntityType EntityType { get; set; }
    public Dictionary<string, string> Data { get; set; } = new();
    
    public string GetValue(string key)
    {
        return Data.TryGetValue(key, out var value) ? value : string.Empty;
    }
    
    public bool HasValue(string key)
    {
        return Data.ContainsKey(key) && !string.IsNullOrWhiteSpace(Data[key]);
    }
}
