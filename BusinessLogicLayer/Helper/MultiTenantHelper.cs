using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BusinessLogicLayer.Helper
{
    public static class MultiTenantHelper
    {
        public static string ToSubdomain(string orgName)
        {
            if (string.IsNullOrWhiteSpace(orgName)) return string.Empty;

            // 1. Handle CamelCase (e.g., "OtherOrg" -> "Other-Org")
            // Inserts a hyphen before capital letters that are not at the start
            string result = Regex.Replace(orgName, @"(?<!^)(?=[A-Z])", "-");

            // 2. Convert to lowercase
            result = result.ToLowerInvariant();

            // 3. Replace all non-alphanumeric characters with hyphens
            result = Regex.Replace(result, @"[^a-z0-9]+", "-");

            // 4. Trim leading/trailing hyphens and remove double hyphens
            result = result.Trim('-');
            result = Regex.Replace(result, @"-+", "-");

            return result;
        }
    }

}
