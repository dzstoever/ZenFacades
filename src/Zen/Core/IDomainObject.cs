using System.Collections.Generic;
using Zen.Core.DomainRules;

namespace Zen.Core
{
    public interface IDomainObject
    {        
        /// <summary>
        /// Adds a business rule to the business object.
        /// </summary>
        /// <param name="rule"></param>
        void AddRule(ValidateRule rule);

        /// <summary>
        /// Determines whether business rules are valid or not.
        /// Creates a list of validation errors when appropriate.
        /// </summary>
        /// <returns></returns>
        bool Validate();

        /// <summary>
        /// Gets list of validations errors.
        /// </summary>
        IList<string> GetValidationErrors();

        /// <summary>
        /// Gets all validation errors combined into 1 string
        /// </summary>
        string GetValidationError();
    }
}