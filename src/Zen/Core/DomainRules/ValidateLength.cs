namespace Zen.Core.DomainRules
{
    /// <summary>
    ///  Length validation rule. 
    ///  Length must be between given min and max values.
    /// </summary>
    public class ValidateLength : ValidateRule
    {
        private readonly int _min;
        private readonly int _max;

        public ValidateLength(string propertyName, int min, int max)
            : base(propertyName)
        {
            _min = min;
            _max = max;

            ErrorMessage = propertyName + " must be between " + _min + " and " + _max + " characters long.";
        }

        public ValidateLength(string propertyName, string errorMessage, int min, int max)
            : this(propertyName, min, max)
        {
            ErrorMessage = errorMessage;
        }

        public override bool Validate(DomainObject domainObject)
        {
            int length = GetPropertyValue(domainObject).ToString().Length;
            return length >= _min && length <= _max;
        }
    }
}
