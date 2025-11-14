using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace AdminDashboard.Infrastructure.Filters
{
    /// <summary>
    /// Global filter to remove sensitive information before sending the response.
    /// Advanced: recursively sanitizes objects and collections.
    /// </summary>
    public class SensitiveDataFilter : IActionFilter
    {
        private static readonly string[] SensitiveFields = new[]
        {
            "password", "token", "refreshtoken", "email", "secretkey", "ssn", "creditcard"
        };

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // No action required before execution
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is not ObjectResult objectResult)
                return;

            if (objectResult.Value is null)
                return;

            // Don't process plain strings or primitives directly returned
            var val = objectResult.Value;
            if (val is string || val.GetType().IsPrimitive)
                return;

            try
            {
                // Convert value to JsonElement safely
                var jsonElement = JsonSerializer.SerializeToElement(val);

                // Only process JSON objects or arrays
                if (jsonElement.ValueKind == JsonValueKind.Object)
                {
                    var sanitized = ConvertJsonElement(jsonElement);
                    objectResult.Value = sanitized;
                }
                else if (jsonElement.ValueKind == JsonValueKind.Array)
                {
                    var sanitizedList = ConvertJsonElement(jsonElement);
                    objectResult.Value = sanitizedList;
                }
            }
            catch
            {
                // Do not break the pipeline if anything goes wrong
                return;
            }
        }

        // Recursively convert JsonElement into CLR types (Dictionary / List / primitive)
        private static object? ConvertJsonElement(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                    foreach (var prop in element.EnumerateObject())
                    {
                        var name = prop.Name;
                        // Skip sensitive fields (case-insensitive)
                        if (IsSensitive(name))
                            continue;

                        dict[name] = ConvertJsonElement(prop.Value);
                    }
                    return dict;

                case JsonValueKind.Array:
                    var list = new List<object?>();
                    foreach (var item in element.EnumerateArray())
                    {
                        list.Add(ConvertJsonElement(item));
                    }
                    return list;

                case JsonValueKind.String:
                    return element.GetString();

                case JsonValueKind.Number:
                    if (element.TryGetInt64(out var l)) return l;
                    if (element.TryGetDouble(out var d)) return d;
                    return element.GetRawText();

                case JsonValueKind.True:
                case JsonValueKind.False:
                    return element.GetBoolean();

                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                default:
                    return null;
            }
        }

        private static bool IsSensitive(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName)) return false;
            return SensitiveFields.Any(s => string.Equals(s, propertyName, StringComparison.OrdinalIgnoreCase)
                                           || propertyName.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}
