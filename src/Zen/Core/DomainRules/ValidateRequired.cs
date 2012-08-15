namespace Zen.Core.DomainRules
{
    /// <summary>
    /// Represents a validation rules that states that a value is required.
    /// </summary>
    public class ValidateRequired : ValidateRule
    {

        public ValidateRequired(string propertyName)
            : base(propertyName)
        {
            ErrorMessage = propertyName + " is a required field.";
        }

        public ValidateRequired(string propertyName, string errorMessage)
            : base(propertyName)
        {
            ErrorMessage = errorMessage;
        }

        public override bool Validate(DomainObject domainObject)
        {
            try
            {
                return GetPropertyValue(domainObject).ToString().Length > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
