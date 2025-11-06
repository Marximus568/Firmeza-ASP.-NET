namespace AdminDashboard.Domain.Entities;

public class Categories
{
    public int Id { get; set; }
        
    public string Name { get; set; } = string.Empty;
        
    public ICollection<Products> Products { get; set; } = new List<Products>();

}