using System.Text.RegularExpressions;

namespace Zen.Core.DomainRules
{
    /// <summary>
    /// Base class for regex based validation rules.
    /// </summary>
    public class ValidateRegex : ValidateRule
    {
        protected string Pattern { get; set; }

        public ValidateRegex(string propertyName, string pattern)
            : base(propertyName)
        {
            Pattern = pattern;
        }

        public ValidateRegex(string propertyName, string errorMessage, string pattern)
            : this(propertyName, pattern)
        {
            ErrorMessage = errorMessage;
        }

        public override bool Validate(DomainObject domainObject)
        {
            return Regex.Match(GetPropertyValue(domainObject).ToString(), Pattern).Success;
        }
    }
}
