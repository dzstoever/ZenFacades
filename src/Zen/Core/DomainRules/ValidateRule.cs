namespace Zen.Core.DomainRules
{
    /// <summary>
    /// Abstract base class for business rules. 
    /// Maintains property name to which rule applies and validation error message.
    /// </summary>
    public abstract class ValidateRule
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }

        /// <param name="propertyName">The property name to which rule applies.</param>
        protected ValidateRule(string propertyName)
        {
            PropertyName = propertyName;
            ErrorMessage = propertyName + " is not valid";
        }

        /// <param name="propertyName">The property name to which rule applies.</param>
        /// <param name="errorMessage">The error message.</param>
        protected ValidateRule(string propertyName, string errorMessage)
            : this(propertyName)
        {
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Validation method. To be implemented in derived classes.
        /// </summary>
        public abstract bool Validate(DomainObject domainObject);

        /// <summary>
        /// Gets value for given business object's property using reflection.
        /// </summary>
        protected object GetPropertyValue(DomainObject domainObject)
        {
            return domainObject.GetType().GetProperty(PropertyName).GetValue(domainObject, null);
        }
    }
}

