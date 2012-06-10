using System;
using FluentAssertions;
using Xbehave;
using Zen.Core;

namespace Zen.Xunit.Tests.Core
{    
    /// <summary>
    /// a base class for any entity test scenarios 
    /// </summary>
    /// <typeparam name="T">entity type</typeparam>
    /// <typeparam name="Tid">entity.Id type</typeparam>
    public abstract class EntityScenarios<T, Tid> : UseLogFixture 
        where T : class, IDomainEntity<Tid>, new()
    {
        protected T _entity;// <- sut 
        protected Tid _id;

        //[Scenario] - add this attribute to the method override
        public virtual void CreateDefault()
        {
            "Given a domain entity".Given(() =>
            {//arrange
                _entity = default(T);
                _id = default(Tid);
                Log.InfoFormat( "{0}\t Entity Type:\t{1} [default= {2}]" + 
                                    "{0}\t Id Type:\t\t{3} [default= {4}] {0}", "\r\n",
                                    typeof(T), _entity.ShowNullorEmptyString(),
                                    typeof(Tid), _id.ShowNullorEmptyString());
            });

            "When instantiating with the default constructor".When(() =>
            {//act
                _entity = Activator.CreateInstance<T>();
            });

            "Then the object should be in the appropriate state".Then(() =>
            {//assert
                _entity.Should()
                    .BeAssignableTo<IDomainEntity<Tid>>("the type should implement IDomainEntity<T>");

                _entity.Id.Should().Be(default(Tid), "the databaase id should not been assigned");

                _entity.Uid.Should().NotBeEmpty("the Guid (EntityId) should be assigned");
            });
        }

        
    }
}