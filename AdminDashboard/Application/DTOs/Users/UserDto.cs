namespace AdminDashboard.Application.Users.DTOs
{
    /// <summary>
    /// Data Transfer Object for user retrieval operations
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// User identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// User's first name
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// User's last name
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// User's full name (computed property)
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// User's email address
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's date of birth
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// User's age (computed from DateOfBirth)
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// User's phone number
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// User's physical address
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// User's role in the system
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Total number of sales associated with this user
        /// </summary>
        public int TotalSales { get; set; }
    }
}