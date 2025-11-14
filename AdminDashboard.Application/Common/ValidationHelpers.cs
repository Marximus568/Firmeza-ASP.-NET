using System.Text.RegularExpressions;

namespace AdminDashboardApplication.Common
{
    /// <summary>
    /// Helper class for additional validation utilities
    /// </summary>
    public static class ValidationHelpers
    {
        /// <summary>
        /// Validates email format using comprehensive regex pattern
        /// </summary>
        /// <param name="email">The email to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Comprehensive email validation pattern
                var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                return Regex.IsMatch(email, emailPattern, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validates phone number format
        /// Accepts formats: +1234567890, (123) 456-7890, 123-456-7890, 1234567890
        /// </summary>
        /// <param name="phoneNumber">The phone number to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            try
            {
                // Pattern for various phone formats
                var phonePattern = @"^[\d\s\-\+\(\)]{7,15}$";
                
                // Check basic pattern
                if (!Regex.IsMatch(phoneNumber, phonePattern))
                    return false;

                // Extract only digits to verify minimum length
                var digitsOnly = Regex.Replace(phoneNumber, @"[^\d]", "");
                return digitsOnly.Length >= 7 && digitsOnly.Length <= 15;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validates if a person is at least the specified age
        /// </summary>
        /// <param name="dateOfBirth">Date of birth</param>
        /// <param name="minimumAge">Minimum required age</param>
        /// <returns>True if meets minimum age, false otherwise</returns>
        public static bool IsMinimumAge(DateTime dateOfBirth, int minimumAge)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;

            if (dateOfBirth.Date > today.AddYears(-age))
            {
                age--;
            }

            return age >= minimumAge;
        }

        /// <summary>
        /// Validates if a name contains only letters and spaces
        /// </summary>
        /// <param name="name">The name to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // Pattern for names (letters, spaces, accented characters)
            var namePattern = @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$";
            return Regex.IsMatch(name, namePattern);
        }

        /// <summary>
        /// Validates if a role is within allowed values
        /// </summary>
        /// <param name="role">The role to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool IsValidRole(string role)
        {
            var allowedRoles = new[] { "Admin", "User", "Manager", "Guest" };
            return allowedRoles.Contains(role, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Sanitizes input by trimming and removing multiple spaces
        /// </summary>
        /// <param name="input">The input to sanitize</param>
        /// <returns>Sanitized string</returns>
        public static string SanitizeInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Trim and replace multiple spaces with single space
            return Regex.Replace(input.Trim(), @"\s+", " ");
        }

        /// <summary>
        /// Normalizes email to lowercase and trims whitespace
        /// </summary>
        /// <param name="email">The email to normalize</param>
        /// <returns>Normalized email</returns>
        public static string NormalizeEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return string.Empty;

            return email.Trim().ToLowerInvariant();
        }

        /// <summary>
        /// Formats phone number to a standard format
        /// </summary>
        /// <param name="phoneNumber">The phone number to format</param>
        /// <returns>Formatted phone number</returns>
        public static string FormatPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return string.Empty;

            // Remove all non-digit characters except +
            var cleaned = Regex.Replace(phoneNumber, @"[^\d\+]", "");
            return cleaned;
        }
    }
}