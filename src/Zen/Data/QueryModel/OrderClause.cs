using System;

namespace Zen.Data.QueryModel
{
    /// <summary>
    /// Represents an order imposed upon a result set. 
    /// </summary>
    [Serializable] 
    public sealed class OrderClause
    {
        public OrderClause(string propertyName, OrderDirections order)
        {
            PropertyName = propertyName;
            Order = order;
        }

        public string PropertyName { get; private set; }

        public OrderDirections Order { get; private set; }

    }
}