using System;
using Zen.Ioc;
using Zen.Log;
using Zen.Svcs;


namespace Zen.Xunit.Tests
{
    public class TestStartupShell : IStartup
    {
        public void Startup()
        {
            WindsorDI.ConfigureFromInstallers = new object[] { new TestInstaller() };
            try 
            {
                Aspects.GetIocDI().Initialize(); // 1. run all installer(s)
            }
            catch (Exception ex)
            {
                "IocDI.Initialize() failed.{0}{1}".LogMe(LogLevel.Fatal, Environment.NewLine, ex.FullMessage());
                throw new ConfigException("Could not initialize dependency injection.", ex);
            }
        }
    }
}