using System;

namespace Zen.Data.QueryModel
{    
    [Serializable] 
    public sealed class Parameter
    {
        public Parameter(string name, object val)
        {
            Name = name;
            Value = val;
        }

        public Parameter(string name, object val, bool isEntity)
        {
            IsEntity = isEntity;
            Name = name;
            Value = val;
        }

        public Parameter(string name, object[] vals)
        {
            IsList = true;
            Name = name;
            Value = vals;
        }
       

        public bool IsEntity { get; set; }

        public bool IsList { get; set; }

        public string Name { get; set; }

        public object Value { get; set; }
    }
}