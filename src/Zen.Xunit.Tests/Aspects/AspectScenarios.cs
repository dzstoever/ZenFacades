using System;
using FluentAssertions;
using Xbehave;
using Zen.Data;
using Zen.Ioc;
using Zen.Log;
using Zen.Xunit.Tests;

namespace Zen.Xunit
{
    public class AspectScenarios : UseLogFixture 
    {        
        ILoggerFactory _loggerFactory;
        IocDI _di;
        IGenericDao _genericDao;
        ISimpleDao _simpleDao;

        [Scenario]
        public virtual void GetInternalImpls()
        {            
            "Given all required dlls".Given(() => 
                Log.Info("All required dll's are referenced."));
                        
            "When using the Aspects".When(() => 
            {
                _loggerFactory = Aspects.GetLoggerFactory();
                _di = Aspects.GetIocDI();
                _genericDao = Aspects.GetGenericDao();
                _simpleDao = Aspects.GetSimpleDao();
            });
            
            "Then all internal implementations are returned".Then(() => 
            {
                _loggerFactory.Should().BeOfType<Log4netLoggerFactory>();
                _di.Should().BeOfType<WindsorDI>();
                _genericDao.Should().BeOfType<NHibernateDao>();
                _simpleDao.Should().BeOfType<NHibernateDao>();

                Log.InfoFormat(
                    "{0}ILoggerFactory  type = {1}" +
                    "{0}IDependencyInj  type = {2}" +
                    "{0}IGenericDao     type = {3}" +
                    "{0}ISimpleDao      type = {4}",
                    Environment.NewLine, _loggerFactory, _di, _genericDao, _simpleDao);
            });            
        }

    }
}
