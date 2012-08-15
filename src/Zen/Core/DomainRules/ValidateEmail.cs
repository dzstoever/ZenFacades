namespace Zen.Core.DomainRules
{
    /// <summary>
    /// Email validation rule.
    /// </summary>
    public class ValidateEmail : ValidateRegex
    {
        public ValidateEmail(string propertyName) :
            base(propertyName, @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")
        {
            ErrorMessage = propertyName + " is not a valid email address";
        }

        public ValidateEmail(string propertyName, string errorMessage) :
            this(propertyName)
        {
            ErrorMessage = errorMessage;
        }
    }
}
