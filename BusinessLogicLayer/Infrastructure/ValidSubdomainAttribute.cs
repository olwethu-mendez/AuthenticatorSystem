using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Infrastructure
{
    public class ValidSubdomainAttribute : ValidationAttribute
    {
        private readonly string[] _reserved = { "admin", "api", "support", "www", "mail", "dev", "test", "qa", "live", "prod", "debug", "beta" };

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string subdomain)
            {
                // Check against blacklist
                if (_reserved.Contains(subdomain.ToLower()))
                {
                    return new ValidationResult("This subdomain is reserved and cannot be used.");
                }

                // Optional: Ensure it matches the slug format (lowercase, hyphens only)
                if (!Regex.IsMatch(subdomain, "^[a-z0-9-]+$"))
                {
                    return new ValidationResult("Subdomains can only contain lowercase letters, numbers, and hyphens.");
                }
            }
            return ValidationResult.Success;
        }
    }

}
