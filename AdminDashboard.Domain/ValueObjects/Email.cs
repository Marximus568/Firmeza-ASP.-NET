using System.Text.RegularExpressions;

namespace AdminDashboard.Domain.ValueObjects
{
    /// <summary>
    /// Value Object representing an email address with validation.
    /// Immutable and self-validating following DDD principles.
    /// </summary>
    public sealed class Email : IEquatable<Email>
    {
        // RFC 5322 simplified regex pattern for email validation
        private static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Factory method to create a validated Email instance.
        /// </summary>
        public static Email Create(string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                throw new ArgumentException("Email address cannot be empty", nameof(emailAddress));
            }

            var normalizedEmail = emailAddress.Trim().ToLowerInvariant();

            if (!EmailRegex.IsMatch(normalizedEmail))
            {
                throw new ArgumentException($"Invalid email format: {emailAddress}", nameof(emailAddress));
            }

            return new Email(normalizedEmail);
        }

        // Value object equality implementation
        public bool Equals(Email other)
        {
            if (other is null) return false;
            return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj) => Equals(obj as Email);

        public override int GetHashCode() => Value.ToLowerInvariant().GetHashCode();

        public override string ToString() => Value;

        public static implicit operator string(Email email) => email.Value;
    }
}