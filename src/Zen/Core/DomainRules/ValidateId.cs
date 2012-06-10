namespace Zen.Core.DomainRules
{
    /// <summary>
    /// Identity validation rule. 
    /// Value must be integer and greater than zero.
    /// </summary>
    public class ValidateId : ValidateRule
    {
        public ValidateId(string propertyName)
            : base(propertyName)
        {
            ErrorMessage = propertyName + " is an invalid identifier";
        }

        public ValidateId(string propertyName, string errorMessage)
            : base(propertyName)
        {
            ErrorMessage = errorMessage;
        }

        public override bool Validate(DomainObject domainObject)
        {
            try
            {
                int id = int.Parse(GetPropertyValue(domainObject).ToString());
                return id >= 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
