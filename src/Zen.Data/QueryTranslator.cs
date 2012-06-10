using System;
using NHibernate;
using NHibernate.Criterion;
using Zen.Data.QueryModel;

namespace Zen.Data
{
    internal class QueryTranslator
    {
        public QueryTranslator(ICriteria criteria, Query query)
        {
            _criteria = criteria;
            _query = query;
        }
          
      
        private ICriteria _criteria;//agnostic criterion becomes NHibernate.ICriteria
        private Junction _junction; //keep track of logical groups of operators
        private Query _query;       //agnostic query from our EIS.Data.QueryModel        
        

        /// <summary>
        /// Translate the criteria for NHibernate 
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void Execute()
        {
            //Add order clauses directly to the ICriteria  
            foreach (var clause in _query.SortOrder)
                _criteria.AddOrder(new Order(clause.PropertyName,
                                             clause.Order == OrderDirections.Ascending));

            foreach (var myCriterion in _query.Criteria)
            {
                ICriterion criterion = null;
                switch (myCriterion.Operator)
                {
                        #region property criteria

                    case CriteriaOperators.Equal:
                        criterion = Restrictions.Eq(myCriterion.PropertyName, myCriterion.Value);
                        break;
                    case CriteriaOperators.NotEqual:
                        criterion = Restrictions.Not(Restrictions.Eq(myCriterion.PropertyName, myCriterion.Value));
                        break;
                    case CriteriaOperators.GreaterThan:
                        criterion = Restrictions.Gt(myCriterion.PropertyName, myCriterion.Value);
                        break;
                    case CriteriaOperators.GreaterThanOrEqual:
                        criterion = Restrictions.Ge(myCriterion.PropertyName, myCriterion.Value);
                        break;
                    case CriteriaOperators.LesserThan:
                        criterion = Restrictions.Lt(myCriterion.PropertyName, myCriterion.Value);
                        break;
                    case CriteriaOperators.LesserThanOrEqual:
                        criterion = Restrictions.Le(myCriterion.PropertyName, myCriterion.Value);
                        break;
                    case CriteriaOperators.Like:
                        criterion = Restrictions.InsensitiveLike(myCriterion.PropertyName, myCriterion.Value);
                        break;
                    case CriteriaOperators.NotLike:
                        criterion =
                            Restrictions.Not(Restrictions.InsensitiveLike(myCriterion.PropertyName, myCriterion.Value));
                        break;
                    case CriteriaOperators.IsNull:
                        criterion = Restrictions.IsNull(myCriterion.PropertyName);
                        break;
                    case CriteriaOperators.IsNotNull:
                        criterion = Restrictions.IsNotNull(myCriterion.PropertyName);
                        break;
                    case CriteriaOperators.In:
                        criterion = Restrictions.In(myCriterion.PropertyName, myCriterion.Values);
                        break;
                    case CriteriaOperators.NotIn:
                        criterion = Restrictions.Not(Restrictions.In(myCriterion.PropertyName, myCriterion.Values));
                        break;

                        #endregion

                        #region logical criteria

                        // we will create a new _junction, nesting the previous _junction group, if necessary                    
                    case CriteriaOperators.And:
                        if (_junction != null)
                        {
                            var prevJunction = _junction;
                            _junction = Restrictions.Conjunction();
                            _junction.Add(prevJunction);
                        }
                        else
                        {
                            _junction = Restrictions.Conjunction();
                        }
                        break;
                    case CriteriaOperators.Or:
                        if (_junction != null)
                        {
                            var prevJunction = _junction;
                            _junction = Restrictions.Disjunction();
                            _junction.Add(prevJunction);
                        }
                        else
                        {
                            _junction = Restrictions.Disjunction();
                        }
                        break;

                        #endregion

                    default:
                        throw new ArgumentException("Operator not supported in NHibernate Provider");
                }

                if (myCriterion.Operator == CriteriaOperators.And || myCriterion.Operator == CriteriaOperators.Or)
                    continue;
                if (_junction == null)
                    _criteria.Add(criterion);
                else
                    _junction.Add(criterion);
            }

            if (_junction != null)
                _criteria.Add(_junction);
        }

    }
}