﻿namespace Zen.Core.DomainRules
{
    /// <summary>
    /// IP Address validation rule.
    /// </summary>
    public class ValidateIPAddress : ValidateRegex
    {
        // Match IP Address
        public ValidateIPAddress(string propertyName) :
            base(propertyName, @"^([0-2]?[0-5]?[0-5]\.){3}[0-2]?[0-5]?[0-5]$")
        {
            ErrorMessage = propertyName + " is not a valid IP Address";
        }

        public ValidateIPAddress(string propertyName, string errorMessage) :
            this(propertyName)
        {
            ErrorMessage = errorMessage;
        }
    }
}
