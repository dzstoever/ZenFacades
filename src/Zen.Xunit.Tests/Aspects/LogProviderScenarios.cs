using System;
using System.Collections.Specialized;
using FluentAssertions;
using Xbehave;
using Xunit;
using Xunit.Extensions;
using Zen.Log;
using Moq;

namespace Zen.Xunit
{
    public class LoggerFactoryProviderScenarios 
    {
        private LogProvider _provider;// <- sut
        private ILoggerFactory _class; // <- result
        private readonly Mock<ImplChecker> _moqChecker = new Mock<ImplChecker>();
        private const string DllName = "log4net.dll";
        private const string KeyName = "log-factory";
        private const string WhenMsg = "When using the LogProvider";


        [Scenario]
        [InlineData(new object[] { true,  typeof(Log4netLoggerFactory) })]  //internal impl
        [InlineData(new object[] { false, typeof(NoLoggerFactory) })]       //default impl
        public virtual void GetImpl_NoSettings(bool dllExists, Type expected)
        {
            _moqChecker.Setup(s => s.CheckForDll(DllName)).Returns(dllExists);

            string.Format("Given dll available = {0} and NO Settings", dllExists).Given(() =>
                _provider = new LogProvider
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
        [InlineData(new object[] { true,  typeof(Log4netLoggerFactory) })]  //internal impl
        [InlineData(new object[] { false, typeof(NoLoggerFactory) })]       //default impl
        public virtual void GetImpl_No_setting_key(bool dllExists, Type expected)
        {
            _moqChecker.Setup(s => s.CheckForDll(DllName)).Returns(dllExists);

            string.Format("Given dll available = {0} and NO '{1}' setting", dllExists, KeyName).Given(() =>
                _provider = new LogProvider
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
        [InlineData(new object[] { true,  "Zen.Tests.DummyLoggerFactory, Zen.Tests", typeof(DummyLoggerFactory) })] //other impl
        [InlineData(new object[] { false, "Zen.Tests.DummyLoggerFactory, Zen.Tests", typeof(DummyLoggerFactory) })] //other impl
        public virtual void GetImpl_Valid_setting(bool dllExists, string setting, Type expected)
        {
            _moqChecker.Setup(s => s.CheckForDll(DllName)).Returns(dllExists);

            string.Format("Given dllAvailable = {0} and setting is [{1}]", dllExists, setting).Given(() =>
                _provider = new LogProvider
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
                _provider = new LogProvider
                {
                    DependencyChecker = _moqChecker.Object,
                    Settings = new NameValueCollection { { KeyName, setting } }
                });

            "Then an dependency exception should be thrown".Then(() =>
                Assert.Throws<DependencyException>(() => _provider.GetImpl())
                );
        }

    }
}
