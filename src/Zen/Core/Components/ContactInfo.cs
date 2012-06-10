using System;
using System.Text;

namespace Zen.Core
{
    public class ContactInfo : DomainObject
    {        
        public virtual NameInfo Name
        {
            get { return _name; }
            set { _name = value; }
        }
        
        public virtual AddressInfo Address
        {
            get { return _address; }
            set { _address = value; }
        }
        
        public virtual PhoneInfo Phone1
        {
            get { return _phone1; }
            set { _phone1 = value; }
        }
        
        public virtual PhoneInfo Phone2
        {
            get { return _phone2; }
            set { _phone2 = value; }
        }
        
        public virtual PhoneInfo Fax
        {
            get { return _fax; }
            set { _fax = value; }
        }
                
        public virtual string Email { get; set; }

        private NameInfo _name = new NameInfo();
        private AddressInfo _address = new AddressInfo();
        private PhoneInfo _phone1 = new PhoneInfo();
        private PhoneInfo _phone2 = new PhoneInfo();
        private PhoneInfo _fax = new PhoneInfo();

        /// <summary>
        /// Checks to see if Name and Address are valid,
        /// ignores Phone, Fax, and Email
        /// </summary>
        /// <remarks>
        /// If false, check the ValidationResults of the 
        /// Name and Address for the specific errors.
        /// </remarks>
        public override bool Validate()
        {
            try
            {
                if (!base.Validate()) return false; //Name or address is null...
                if (Name == null) return false;
                if (Address == null) return false;
                //validate name and address members 
                return (Name.Validate() && Address.Validate());
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public override string GetValidationError()
        {
            if (base.Validate()
                    && Name != null && Name.Validate()
                    && Address != null && Address.Validate())
                //Validation succeeded.
                return base.GetValidationError();

            //Validation failed.
            var sb = new StringBuilder();
            if (!base.Validate())
                sb.AppendFormat("{0}{1}", Environment.NewLine, base.GetValidationError());
            if (Name != null && !Name.Validate())
                sb.AppendFormat("{0}{1}", Environment.NewLine, Name.GetValidationError());
            if (Address != null && !Address.Validate())
                sb.AppendFormat("{0}{1}", Environment.NewLine, Address.GetValidationError());
            return sb.ToString();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Name != null)       sb.AppendLine(Name.ToString());
            if (Address != null)    sb.AppendLine(Address.MultilineDisplay);
            if (Phone1 != null)     sb.AppendLine("Phone1: " + Phone1);
            if (Phone2 != null)     sb.AppendLine("Phone2: " + Phone2);
            if (Fax != null)        sb.AppendLine("Fax: " + Fax);
            if (Email != null)      sb.AppendLine("Email: " + Email);
            return sb.ToString();
        }

    }
}