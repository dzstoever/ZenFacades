using System;
using System.Collections.Specialized;
using FluentAssertions;
using Xbehave;
using Xunit;
using Xunit.Extensions;
using Zen.Ioc;
using Moq;

namespace Zen.Xunit
{
    public class IocDIProviderScenarios 
    {
        private IocProvider _provider;// <- sut
        private IocDI _class;// <- result
        private readonly Mock<ImplChecker> _moqChecker = new Mock<ImplChecker>();
        private const string DllName = "Castle.Windsor.dll";
        private const string KeyName = "ioc-di";
        private const string WhenMsg = "When using the IocProvider";


        [Scenario]
        [InlineData(new object[] { true,  typeof(WindsorDI) })]     //internal impl
        [InlineData(new object[] { false, typeof(SingletonDI) })]   //default impl
        public virtual void GetImpl_NoSettings(bool dllExists, Type expected)
        {
            _moqChecker.Setup(s => s.CheckForDll(DllName)).Returns(dllExists);

            string.Format("Given dll available = {0} and NO Settings", dllExists).Given(() =>
                _provider = new IocProvider
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
        [InlineData(new object[] { true,  typeof(WindsorDI) })]     //internal impl
        [InlineData(new object[] { false, typeof(SingletonDI) })]   //default impl
        public virtual void GetImpl_No_setting_key(bool dllExists, Type expected)
        {
            _moqChecker.Setup(s => s.CheckForDll(DllName)).Returns(dllExists);

            string.Format("Given dll available = {0} and NO '{1}' setting", dllExists, KeyName).Given(() =>
                _provider = new IocProvider
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
        [InlineData(new object[] { true,  "Zen.Tests.DummyDI, Zen.Tests", typeof(DummyDI) })] //other impl
        [InlineData(new object[] { false, "Zen.Tests.DummyDI, Zen.Tests", typeof(DummyDI) })] //other impl
        public virtual void GetImpl_Valid_setting(bool dllExists, string setting, Type expected)
        {
            _moqChecker.Setup(s => s.CheckForDll(DllName)).Returns(dllExists);
            
            string.Format("Given dllAvailable = {0} and setting is [{1}]", dllExists, setting).Given(() =>
                _provider = new IocProvider
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
                _provider = new IocProvider 
                { 
                    DependencyChecker = _moqChecker.Object,
                    Settings = new NameValueCollection { { KeyName, setting } }
                });

            "Then an dependency exception should be thrown".Then(() =>
                Assert.Throws<DependencyException>(() => _provider.GetImpl() )
                );
        }


    }
}
