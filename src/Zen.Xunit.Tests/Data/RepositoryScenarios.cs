using Xbehave;
using FluentAssertions;
using System;
using System.Collections.Generic;
using NHibernate;
using Zen.Core;
using Zen.Data;

namespace Zen.Xunit.Tests.Data
{
    /// <summary>
    /// Base class for any repository entity tests,
    /// creates a generic dao object to access the data. 
    /// It is automapped using FluentNHibernate.
    /// </summary>
    /// <remarks>
    /// /* Why test using a Mock Database instead of Mock Objects?
    ///  * 
    ///  * When testing RDBMS  we generally want to test only three things, 
    ///  *  that properties are persisted, 
    ///  *  that cascade works as expected,
    ///  *  and that queries return the correct result. 
    ///  * In order to do all of those, we generally have to talk to a real database, 
    ///  *  trying to fake any of those at this level is futile and going to be very complicated.
    ///  * We can will use a standard in memory database(SQLite) in order to get very speedy tests.
    ///  * (For NHibernate) We can let FluentNHibernate give us a standard configuration. */
    /// </remarks>
    /// <see cref="http://ayende.com/blog/3983/nhibernate-unit-testing"/>
    /// <typeparam name="T">type of the entity</typeparam>
    /// <typeparam name="Tid">type of the database identifir for the entity</typeparam>
    public abstract class RepositoryScenarios<T, Tid> : UseLogFixture 
        where T : class, IDomainEntity<Tid>, new()
    {
        protected IGenericDao _dao;// <- sut (for entities of type T in the repository)
        protected IDisposable _session;        
        int _entityCnt = -1;
        IList<T> _entityList;

        #region some unused mocking code that may be useful for Save() or Update() calls
        //protected Mock<NH.ISessionFactory> _mockSessionFactory = new Mock<NH.ISessionFactory>();
        //protected Mock<NH.ISession> _mockSession = new Mock<NH.ISession>();        

        // tell the mock session factory to return a mock session
        //_mockSessionFactory.Setup(sf => sf.OpenSession()).Returns(_mockSession.Object);            
        //_dao = new NHibernate.GenericDao(_mockSessionFactory.Object);
        #endregion

        [Scenario]
        public virtual void FetchAll(ISessionFactory sessionFactory)
        {

            "Given a generic data access object with an open unit of work".Given(() =>
            {//arrange
                
                _dao = new NHibernateDao(sessionFactory);
                _session = _dao.StartUnitOfWork();

                Log.InfoFormat("Testing repository with {0}", _dao.GetType());
            }, Dispose
            );

            "When fetching entities from the repository".When(() =>
            {//act
                _entityCnt = _dao.GetCount<T>();
                _entityList = _dao.FetchAll<T>();

                Log.InfoFormat("{0} count = {1}", typeof(T), _entityCnt);
            });

            "Then all entities should be returned".Then(() =>
            {//assert
                _session.Should().BeAssignableTo<ISession>("an NHibernate.ISession should be available");

                _entityList.Should().NotBeNull("a valid list should be returned");

                _entityList.Count.Should().Be(_entityCnt, "the number of instances should equal the total count");

                foreach (var entity in _entityList)
                {
                    entity.Id.Should().NotBe(default(Tid), "each instance should have an Id assigned");
                }

            });
        }
        
        public void Dispose()
        {
            if (_dao == null) return;            
            _dao.CloseUnitOfWork();
            _dao.Dispose();
        }
    }
}