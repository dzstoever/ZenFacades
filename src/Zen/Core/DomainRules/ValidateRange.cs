using System;

namespace Zen.Core.DomainRules
{
    /// <summary>
    /// Validates a range (min and max) for a given data type.
    /// </summary>
    public class ValidateRange : ValidateRule
    {
        private ValidationDataType DataType { get; set; }
        private ValidationOperator Operator { get; set; }

        private object Min { get; set; }
        private object Max { get; set; }

        public ValidateRange(string propertyName, object min, object max,
            ValidationOperator @operator, ValidationDataType dataType)
            : base(propertyName)
        {
            Min = min;
            Max = max;

            Operator = @operator;
            DataType = dataType;

            ErrorMessage = propertyName + " must be between " + Min + " and " + Max;
        }

        public ValidateRange(string propertyName, string errorMessage, object min, object max,
            ValidationOperator @operator, ValidationDataType dataType)
            : this (propertyName,  min,  max,@operator, dataType)
        {
            ErrorMessage = errorMessage;
        }

        public override bool Validate(DomainObject domainObject)
        {
            try
            {
                string value = GetPropertyValue(domainObject).ToString();

                switch (DataType)
                {
                    case ValidationDataType.Integer:

                        var imin = int.Parse(Min.ToString());
                        var imax = int.Parse(Max.ToString());
                        var ival = int.Parse(value);

                        return (ival >= imin && ival <= imax);

                    case ValidationDataType.Double:
                        var dmin = double.Parse(Min.ToString());
                        var dmax = double.Parse(Max.ToString());
                        var dval = double.Parse(value);

                        return (dval >= dmin && dval <= dmax);

                    case ValidationDataType.Decimal:
                        var cmin = decimal.Parse(Min.ToString());
                        var cmax = decimal.Parse(Max.ToString());
                        var cval = decimal.Parse(value);

                        return (cval >= cmin && cval <= cmax);

                    case ValidationDataType.Date:
                        var tmin = DateTime.Parse(Min.ToString());
                        var tmax = DateTime.Parse(Max.ToString());
                        var tval = DateTime.Parse(value);

                        return (tval >= tmin && tval <= tmax);

                    case ValidationDataType.String:

                        var smin = Min.ToString();
                        var smax = Max.ToString();

                        var result1 = String.CompareOrdinal(smin, value);
                        var result2 = String.CompareOrdinal(value, smax);

                        return result1 >= 0 && result2 <= 0;
                }
                return false;
            }
            catch { return false; }
        }
    }
}
