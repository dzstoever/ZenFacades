using System.Text;

namespace Zen.Core
{
    public class NameInfo : DomainObject
    {
        public NameInfo()
        { }

        public NameInfo(string fullName)
        {
            _fullName = fullName;
            SplitFullName();           
        }

        public virtual string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(_fullName)) return _fullName;

                var sb = new StringBuilder();
                if (!string.IsNullOrEmpty(First)) sb.Append(First + " ");
                if (!string.IsNullOrEmpty(Middle)) sb.Append(Middle + " ");
                if (!string.IsNullOrEmpty(Last)) sb.Append(Last);
                return sb.ToString();
            }
            set
            {
                _fullName = value;
                SplitFullName();
            }
        }
        private string _fullName;

        public virtual string First 
        { 
            get { return _first; }
            set { _first = value; } 
        }
        private string _first;

        public virtual string Middle
        {
            get { return _middle; }
            set { _middle = value; }
        }
        private string _middle;

        public virtual char MiddleInit
        {
            get
            {
                return string.IsNullOrEmpty(Middle) ? ' ' : Middle.Trim()[0];
            }
        }

        public virtual string Last
        {
            get { return _last; }
            set { _last = value; }
        }
        private string _last;


        private void SplitFullName()
        {
            var parts = _fullName.Split(new[] { ' ' });
            if (parts.Length == 1)
            { 
                _last = parts[0]; 
            }
            else if (parts.Length == 2)
            {
                _first = parts[0];
                _last = parts[1];
            }
            else if (parts.Length == 3 && parts[1].Length == 1)
            {
                _first = parts[0];
                _middle = parts[1];
                _last = parts[2];
            }
            else if (parts.Length >= 3)
            {
                _first = parts[0];
                if(parts[1].Length == 1)
                    _middle = parts[1];

                var startAt = parts[1].Length == 1 ? 2 : 1;
                _last = parts[startAt];
                for (var i = startAt+1; i < parts.Length; i++)
                    _last += " " + parts[i];
            }
        }

        public override string ToString()
        {
            return Validate() ? FullName : GetValidationError();
        }
        
    }
}