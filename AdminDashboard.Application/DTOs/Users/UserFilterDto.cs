namespace AdminDashboardApplication.DTOs.Users;

public class UserFilterDto
{
    public string? SearchTerm { get; set; }
    public string? Role { get; set; }
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public string? EmailDomain { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "FirstName";
    public string SortDirection { get; set; } = "asc";
}

