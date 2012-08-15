namespace Zen.Core
{
    public class PhoneInfo : DomainObject
    {        
        public virtual string Number { get; set; }
        
        public override string ToString()
        {
            var display = Number.Length != 10 ? Number
                : Number.Insert(0, "(").Insert(4, ") ").Insert(9, "-");

            return Validate() ? display : GetValidationError();
        }

    }
}