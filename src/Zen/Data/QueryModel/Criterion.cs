using System;

namespace Zen.Data.QueryModel
{
    [Serializable]
    public sealed class Criterion 
    {
        public Criterion()
        {
        }

        public Criterion(string propertyName, CriteriaOperators @operator, object value) : this()
        {
            PropertyName = propertyName;
            Operator = @operator;
            Value = value;
        }

        public Criterion(string propertyName, CriteriaOperators @operator, object[] values) : this()
        {
            PropertyName = propertyName;
            Operator = @operator;
            Values = values;
        }
        

        public CriteriaOperators Operator { get; set; }
        
        public string PropertyName { get; set; }

        public object Value { get; set; }

        public object[] Values { get; set; }

    }
}