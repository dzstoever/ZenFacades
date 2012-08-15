using System;
using System.Text;

namespace Zen.Core
{
    public class AddressInfo : DomainObject
    {        
        public virtual string Street1 { get; set; }
        public virtual string Street2 { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string Zip5 { get; set; }
        public virtual string Zip4 { get; set; }
        
        /// <summary>
        /// combines or splits (Zip5-Zip4), *
        /// must set with '-' seperator
        /// </summary>        
        public string PostalCode
        {
            get 
            { 
                return string.IsNullOrEmpty(Zip4) ? Zip5 : String.Format("{0}-{1}", Zip5, Zip4); 
            }
            set
            {
                var parts = value.Split(new[] {'-'});
                Zip5 = parts[0].Trim();
                if (parts.Length > 1) Zip4 = parts[1].Trim();
            }
        }

        /// <summary>
        /// combines Street1 + Street2 (if avalaible),
        /// each on it's own line
        /// </summary>
        public string MultilineStreet
        {
            get
            {
                return string.IsNullOrEmpty(Street2) ? Street1 : Street1 + Environment.NewLine + Street2;
            }
        }

        /// <summary>
        /// Combines Street1, Street2, & City, ST PostalCode,
        /// each on it's own line
        /// </summary>
        public string MultilineDisplay
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine(MultilineStreet);
                sb.Append(string.Format("{0}, {1}  {2}", City, State, PostalCode));
                return Validate() ? sb.ToString() : GetValidationError();
            }
        }
        

        public override string ToString()
        {
            return Validate() ? MultilineDisplay.Replace(Environment.NewLine, " ~ ") : GetValidationError();
        }
        
    }
}