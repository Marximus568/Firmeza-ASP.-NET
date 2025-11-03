namespace AdminDashboard.Application.Users.DTOs
{
    /// <summary>
    /// Data Transfer Object for filtering user search operations
    /// </summary>
    public class UserFilterDto
    {
        /// <summary>
        /// Search term for filtering by first name, last name, or email
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Filter by specific role
        /// </summary>
        public string? Role { get; set; }

        /// <summary>
        /// Filter by minimum age
        /// </summary>
        public int? MinAge { get; set; }

        /// <summary>
        /// Filter by maximum age
        /// </summary>
        public int? MaxAge { get; set; }

        /// <summary>
        /// Filter by email domain (e.g., "gmail.com")
        /// </summary>
        public string? EmailDomain { get; set; }

        /// <summary>
        /// Page number for pagination (default: 1)
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Page size for pagination (default: 10)
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Sort field (FirstName, LastName, Email, DateOfBirth, Role)
        /// </summary>
        public string SortBy { get; set; } = "FirstName";

        /// <summary>
        /// Sort direction (asc, desc)
        /// </summary>
        public string SortDirection { get; set; } = "asc";
    }
}