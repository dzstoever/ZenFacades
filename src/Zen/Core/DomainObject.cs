using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zen.Core.DomainRules;

namespace Zen.Core
{
    /// <summary>
    /// Abstract base class for business objects.
    /// Contains basic business rule infrastructure.
    /// </summary>
    public abstract class DomainObject : IDomainObject
    {   
        private readonly IList<ValidateRule> _businessRules = new List<ValidateRule>();
        private readonly IList<string> _validationErrors = new List<string>();


        /// <summary>
        /// Adds a business rule to the business object.
        /// </summary>
        /// <param name="rule"></param>
        public virtual void AddRule(ValidateRule rule)
        {
            _businessRules.Add(rule);
        }

        /// <summary>
        /// Determines whether business rules are valid or not.
        /// Creates a list of validation errors when appropriate.
        /// </summary>
        /// <returns></returns>
        public virtual bool Validate()
        {
            var isValid = true;

            _validationErrors.Clear();

            foreach (var rule in _businessRules.Where(rule => !rule.Validate(this)))
            {
                isValid = false;
                _validationErrors.Add(rule.ErrorMessage);
            }
            return isValid;
        }


        /// <summary>
        /// Gets list of validations errors.
        /// </summary>
        public virtual IList<string> GetValidationErrors()
        {
            return _validationErrors;
        }

        /// <summary>
        /// Gets all validation errors combined into 1 string
        /// </summary>
        public virtual string GetValidationError()
        {
            var sb = new StringBuilder("Invalid! ");
            foreach (var validationError in GetValidationErrors())
                sb.AppendLine(validationError);

            return sb.ToString();
        }
    }
}