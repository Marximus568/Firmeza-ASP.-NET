using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace AdminDashboard.Infrastructure.Filters
{
    /// <summary>
    /// Global filter to remove sensitive information before sending the response.
    /// </summary>
    public class SensitiveDataFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // No action required before the controller executes.
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Check if the result is an ObjectResult (like Ok(), Created(), etc.)
            if (context.Result is ObjectResult objectResult && objectResult.Value != null)
            {
                // Convert the object to a dictionary for inspection
                var json = JsonSerializer.Serialize(objectResult.Value);
                var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                if (dict == null) return;

                // List of fields that should be removed from the response
                string[] sensitiveFields = { "Password", "Token", "Email", "RefreshToken", "SecretKey" };

                foreach (var field in sensitiveFields)
                {
                    if (dict.ContainsKey(field))
                    {
                        dict.Remove(field);
                    }
                }

                // Replace the response value with the sanitized object
                objectResult.Value = dict;
            }
        }
    }
}