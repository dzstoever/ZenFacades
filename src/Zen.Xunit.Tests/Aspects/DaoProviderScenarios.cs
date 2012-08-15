using System;
using System.Collections.Specialized;
using FluentAssertions;
using Xbehave;
using Xunit;
using Xunit.Extensions;
using Zen.Data;
using Moq;

namespace Zen.Xunit
{
    public class GenericDaoProviderScenarios 
    {        
        private GenericDaoProvider _provider;// <-- sut
        private IGenericDao _class; // <- result
        private readonly Mock<ImplChecker> _moqChecker = new Mock<ImplChecker>();
        private const string DllName = "Zen.Data.dll";
        private const string KeyName = "dao-generic";
        private const string WhenMsg = "When using the GenericDaoProvider";


        [Scenario]
        [InlineData(new object[] { true,  typeof(NHibernateDao) })]     //internal impl
        //[InlineData(new object[] { false, null })]   //no default impl
        public virtual void GetImpl_NoSettings(bool dllExists, Type expected)
        {
            _moqChecker.Setup(s => s.CheckForDll(DllName)).Returns(dllExists);

            string.Format("Given dll available = {0} and NO Settings", dllExists).Given(() =>
                _provider = new GenericDaoProvider
                {
                    DependencyChecker = _moqChecker.Object,
                    Settings = null//<- no settings
                });

            WhenMsg.When(() =>
                _class = _provider.GetImpl());

            string.Format("Then a {0} should be returned", expected.Name).Then(() =>
               _class.GetType().Should().Be(expected));

        }


        [Scenario]
        [InlineData(new object[] { true,  typeof(NHibernateDao) })]     //internal impl
        //[InlineData(new object[] { false, null })]   //no default impl
        public virtual void GetImpl_No_setting_key(bool dllExists, Type expected)
        {
            _moqChecker.Setup(s => s.CheckForDll(DllName)).Returns(dllExists);

            string.Format("Given dll available = {0} and NO '{1}' setting", dllExists, KeyName).Given(() =>
                _provider = new GenericDaoProvider
                {
                    DependencyChecker = _moqChecker.Object,
                    Settings = new NameValueCollection()// <- don't contain the key
                });

            WhenMsg.When(() =>
                _class = _provider.GetImpl());

            string.Format("Then a {0} should be returned", expected.Name).Then(() =>
                _class.GetType().Should().Be(expected));

        }


        [Scenario]
        [InlineData(new object[] { true,  "Zen.Tests.DummyDao, Zen.Tests", typeof(DummyDao) })] //other impl
        [InlineData(new object[] { false, "Zen.Tests.DummyDao, Zen.Tests", typeof(DummyDao) })] //other impl
        public virtual void GetImpl_Valid_setting(bool dllExists, string setting, Type expected)
        {
            _moqChecker.Setup(s => s.CheckForDll(DllName)).Returns(dllExists);

            string.Format("Given dllAvailable = {0} and setting is [{1}]", dllExists, setting).Given(() =>
                _provider = new GenericDaoProvider
                {
                    DependencyChecker = _moqChecker.Object,
                    Settings = new NameValueCollection { { KeyName, setting } }
                });

            WhenMsg.When(() =>
                _class = _provider.GetImpl());

            string.Format("Then a {0} should be returned", expected.Name).Then(() =>
                _class.GetType().Should().Be(expected));
        }


        [Scenario]
        [InlineData(new object[] { true,  "Some Invalid Type" })]
        [InlineData(new object[] { false, "Some Invalid Type" })]
        public virtual void GetImpl_Invalid_setting(bool dllExists, string setting)
        {
            _moqChecker.Setup(s => s.CheckForDll(DllName)).Returns(dllExists);

            string.Format("Given dllAvailable = {0} and setting is [{1}]", dllExists, setting).Given(() =>
                _provider = new GenericDaoProvider
                {
                    DependencyChecker = _moqChecker.Object,
                    Settings = new NameValueCollection { { KeyName, setting } }
                });

            "Then an dependency exception should be thrown".Then(() =>
                Assert.Throws<DependencyException>(() => _provider.GetImpl())
                );
        }
        
                
        [Scenario]
        public virtual void GetDefaultImpl_ShouldBeNull()
        {
            _moqChecker.Setup(s => s.CheckForDll("Zen.Data.dll")).Returns(false);

            "Given no setting override and Zen.Data.dll is NOT available".Given(() => 
                _provider = new GenericDaoProvider() 
                { 
                    Settings = null, 
                    DependencyChecker = _moqChecker.Object 
                });
            
            WhenMsg.When(() => 
                _class = _provider.GetImpl() );

            "Then null should be returned".Then(() => 
                _class.Should().BeNull());            
        }
        

    }


    public class SimpleDaoProviderScenarios
    {
        private SimpleDaoProvider _provider;// <-- sut
        private ISimpleDao _class; // <- result
        private readonly Mock<ImplChecker> _moqChecker = new Mock<ImplChecker>();
        private const string DllName = "Zen.Data.dll";
        private const string KeyName = "dao-simple";
        private const string WhenMsg = "When using the SimpleDaoProvider";


        [Scenario]
        [InlineData(new object[] { true, typeof(NHibernateDao) })]     //internal impl
        //[InlineData(new object[] { false, null })]   //no default impl
        public virtual void GetImpl_NoSettings(bool dllExists, Type expected)
        {
            _moqChecker.Setup(s => s.CheckForDll(DllName)).Returns(dllExists);

            string.Format("Given dll available = {0} and NO Settings", dllExists).Given(() =>
                _provider = new SimpleDaoProvider
                {
                    DependencyChecker = _moqChecker.Object,
                    Settings = null//<- no settings
                });

            WhenMsg.When(() =>
                _class = _provider.GetImpl());

            string.Format("Then a {0} should be returned", expected.Name).Then(() =>
               _class.GetType().Should().Be(expected));

        }


        [Scenario]
        [InlineData(new object[] { true, typeof(NHibernateDao) })]     //internal impl
        //[InlineData(new object[] { false, null })]   //no default impl
        public virtual void GetImpl_No_setting_key(bool dllExists, Type expected)
        {
            _moqChecker.Setup(s => s.CheckForDll(DllName)).Returns(dllExists);

            string.Format("Given dll available = {0} and NO '{1}' setting", dllExists, KeyName).Given(() =>
                _provider = new SimpleDaoProvider
                {
                    DependencyChecker = _moqChecker.Object,
                    Settings = new NameValueCollection()// <- don't contain the key
                });

            WhenMsg.When(() =>
                _class = _provider.GetImpl());

            string.Format("Then a {0} should be returned", expected.Name).Then(() =>
                _class.GetType().Should().Be(expected));

        }


        [Scenario]
        [InlineData(new object[] { true,  "Zen.Tests.DummyDao, Zen.Tests", typeof(DummyDao) })] //other impl
        [InlineData(new object[] { false, "Zen.Tests.DummyDao, Zen.Tests", typeof(DummyDao) })] //other impl
        public virtual void GetImpl_Valid_setting(bool dllExists, string setting, Type expected)
        {
            _moqChecker.Setup(s => s.CheckForDll(DllName)).Returns(dllExists);

            string.Format("Given dllAvailable = {0} and setting is [{1}]", dllExists, setting).Given(() =>
                _provider = new SimpleDaoProvider
                {
                    DependencyChecker = _moqChecker.Object,
                    Settings = new NameValueCollection { { KeyName, setting } }
                });

            WhenMsg.When(() =>
                _class = _provider.GetImpl());

            string.Format("Then a {0} should be returned", expected.Name).Then(() =>
                _class.GetType().Should().Be(expected));
        }


        [Scenario]
        [InlineData(new object[] { true,  "Some Invalid Type" })]
        [InlineData(new object[] { false, "Some Invalid Type" })]
        public virtual void GetImpl_Invalid_setting(bool dllExists, string setting)
        {
            _moqChecker.Setup(s => s.CheckForDll(DllName)).Returns(dllExists);

            string.Format("Given dllAvailable = {0} and setting is [{1}]", dllExists, setting).Given(() =>
                _provider = new SimpleDaoProvider
                {
                    DependencyChecker = _moqChecker.Object,
                    Settings = new NameValueCollection { { KeyName, setting } }
                });

            "Then an dependency exception should be thrown".Then(() =>
                Assert.Throws<DependencyException>(() => _provider.GetImpl())
                );
        }


        [Scenario]
        public virtual void GetDefaultImpl_ShouldBeNull()
        {
            _moqChecker.Setup(s => s.CheckForDll("Zen.Data.dll")).Returns(false);

            "Given no setting override and Zen.Data.dll is NOT available".Given(() =>
                _provider = new SimpleDaoProvider()
                {
                    Settings = null,
                    DependencyChecker = _moqChecker.Object
                });

            WhenMsg.When(() =>
                _class = _provider.GetImpl());

            "Then null should be returned".Then(() =>
                _class.Should().BeNull());
        }


    }

}
