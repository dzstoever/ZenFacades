using System;
using System.Collections.Generic;

namespace Zen.Data.QueryModel
{
    /* Note: 
     * To pass a Query object as part of a [DataContract] we had to decorate 
     * Criterion, Parameter, and OrderClause classes with the [Serializable]
     * attribute to avoid a System.Runtime.Serialization.InvalidDataContractException 
    
    [DataContract(Namespace = "http://zen.data/types/")]*/
    public sealed class Query
    {
        /// <summary>QueryType = QueryTypes.Criteria
        /// </summary>
        public Query()
        {
            QueryType = QueryTypes.Criteria; 
        }

        public Query(QueryTypes qType)
        {
            QueryType = qType;
        }

        public Query(QueryTypes qType, string text)
        {
            if (qType == QueryTypes.Criteria)
                throw new ArgumentException("QueryType can not be 'Criteria' for Named or Text Queries.");            

            QueryType = qType;
            IsNamed = false;
            NameOrText = text;
        }

        public Query(QueryTypes qType, bool isNamed, string nameOrText)
        {
            if (qType == QueryTypes.Criteria)
                throw new ArgumentException("QueryType can not be 'Criteria' for Named or Text Queries.");            
            
            QueryType = qType;
            IsNamed = isNamed;
            NameOrText = nameOrText;
        }

        
        public bool IsNamed { get; set; }
        
        public string NameOrText { get; set; }
        
        public QueryTypes QueryType { get; set; }

        /// <summary>
        ///     for ICriteria queries only
        /// </summary>
        public IList<Criterion> Criteria = new List<Criterion>();

        /// <summary>
        ///     for ICriteria queries only
        /// </summary>     
        public IList<OrderClause> SortOrder = new List<OrderClause>();

        /// <summary>
        ///     for ICriteria and IQuery(Hql) queries
        /// </summary>
        public IList<Parameter> Parameters = new List<Parameter>();

        /// <summary>
        ///     for ISqlQuery(Sql) queries only
        /// </summary>
        public IDictionary<string, Type> EntityAliases = new Dictionary<string, Type>();

        
        #region Not Implemented
        
        /*
        public void AddCriteria() 
        { }
        public void AddOrderClause() 
        { }
        public void AddParameter() 
        { }
        public void AddEntityAlias() 
        { }
        */

        #endregion

    }
}