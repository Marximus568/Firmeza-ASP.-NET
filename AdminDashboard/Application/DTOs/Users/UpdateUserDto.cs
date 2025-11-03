using System.ComponentModel.DataAnnotations;

namespace AdminDashboard.Application.Users.DTOs
{
    /// <summary>
    /// Data Transfer Object for updating an existing user
    /// </summary>
    public class UpdateUserDto
    {
        /// <summary>
        /// User identifier
        /// </summary>
        [Required(ErrorMessage = "User ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid user ID")]
        public int Id { get; set; }

        /// <summary>
        /// User's first name
        /// </summary>
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "First name can only contain letters")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// User's last name
        /// </summary>
        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Last name can only contain letters")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// User's email address
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's date of birth
        /// </summary>
        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        [MinimumAge(18, ErrorMessage = "User must be at least 18 years old")]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// User's phone number
        /// </summary>
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(15, MinimumLength = 7, ErrorMessage = "Phone number must be between 7 and 15 characters")]
        [RegularExpression(@"^[\d\s\-\+\(\)]+$", ErrorMessage = "Phone number can only contain digits, spaces, and symbols: - + ( )")]
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// User's physical address
        /// </summary>
        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 200 characters")]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// User's role in the system (Admin, User, etc.)
        /// </summary>
        [Required(ErrorMessage = "Role is required")]
        [StringLength(20, ErrorMessage = "Role cannot exceed 20 characters")]
        [RegularExpression(@"^(Admin|User|Manager|Guest)$", ErrorMessage = "Invalid role. Allowed values: Admin, User, Manager, Guest")]
        public string Role { get; set; } = "User";
    }
}
public class MinimumAgeAttribute : ValidationAttribute
{
    private readonly int _minimumAge;

    public MinimumAgeAttribute(int minimumAge)
    {
        _minimumAge = minimumAge;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime dateOfBirth)
        {
            var age = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > DateTime.Today.AddYears(-age))
            {
                age--;
            }

            if (age >= _minimumAge)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage ?? $"Minimum age requirement is {_minimumAge} years");
        }

        return new ValidationResult("Invalid date format");
    }
}
