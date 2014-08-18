using Xbehave;
using Xunit;
using Xunit.Extensions;
using FluentAssertions;
using System;
using System.Configuration;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.IO;
using NHibernate;
using Zen.Quartz.Automap;
using Zen.Xunit.Tests;

namespace Zen.Quartz.Xunit
{

    public class QuartzDatabaseScenarios : UseLogFixture
    {
        public const string MsSqlAppSettingsCnnKey = "cnnTest";
        public const string SqLiteDbName = "QuartzDb";
        

        [Scenario]                
        [InlineData(new object[] { MsSqlAppSettingsCnnKey, true, "QuartzDb.MsSql2008Mappings" })]
        [InlineData(new object[] { MsSqlAppSettingsCnnKey, true, null })]
        [InlineData(new object[] { MsSqlAppSettingsCnnKey, false, null })]
        public void CreateMsSql2008SessionFactory(string appSettingsCnnKey, bool exportDbSchema, string exportMappingsTo)
        {
            var sessionFactory = default(ISessionFactory);

            "Given valid input parameters".Given(() =>
                //arrange                
                Log.InfoFormat("{0}\t AppSettingsCnnKey:\t{1} {0}\t ExportDbSchema:\t{2} {0}\t ExportMappingsTo:\t{3} {0}",
                    Environment.NewLine, appSettingsCnnKey, exportDbSchema, exportMappingsTo ?? "-"),

                //dispose action - runs after Then()
                () => { if (sessionFactory != null) sessionFactory.Dispose(); });

            "When a MsSql2008 session factory is requested".When(() =>
            {//act
                sessionFactory =
                    QuartzDbAutomap.GetMsSql2008SessionFactory(appSettingsCnnKey, exportDbSchema, exportMappingsTo);
            });

            "Then the configuration should succeeed".Then(() =>
            {//assert
                sessionFactory.Should().NotBeNull("a session factory should be created");

                if (string.IsNullOrEmpty(exportMappingsTo)) return;
                var exportDir = exportMappingsTo.Contains(@"\") ? new DirectoryInfo(exportMappingsTo)
                                    : new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, exportMappingsTo));
                exportDir.GetFiles("*.hbm.xml")
                    .Length.Should().BePositive("there should be mapping files");
            });
        }


        [Scenario]
        [InlineData(new object[] { SqLiteDbName, true, "QuartzDb.SqLiteMappings" })]
        [InlineData(new object[] { SqLiteDbName, true, null })]
        [InlineData(new object[] { SqLiteDbName, false, null })]
        public void CreateSqLiteSessionFactory(string dbName, bool exportDbSchema, string exportMappingsTo)
        {
            var sessionFactory = default(ISessionFactory);

            "Given valid input parameters".Given(() =>
             //arrange                
                Log.InfoFormat("{0}\t DatabaseName:\t\t{1} {0}\t ExportDbSchema:\t{2} {0}\t ExportMappingsTo:\t{3} {0}",
                    Environment.NewLine, dbName, exportDbSchema, exportMappingsTo ?? "-"),

                //dispose action - runs after Then()
                () => { if (sessionFactory != null) sessionFactory.Dispose(); });

            "When a SqLite session factory is requested".When(() =>
            {//act
                sessionFactory = 
                    QuartzDbAutomap.GetSqLiteSessionFactory(dbName, exportDbSchema, exportMappingsTo);
            });

            "Then the configuration should succeeed".Then(() =>
            {//assert
                sessionFactory.Should().NotBeNull("a session factory should be created");
                
                File.Exists(dbName).Should().BeTrue("the database file should exist");//even if exportDbSchema = false

                if (string.IsNullOrEmpty(exportMappingsTo)) return;
                var exportDir = exportMappingsTo.Contains(@"\") ? new DirectoryInfo(exportMappingsTo)
                                    : new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, exportMappingsTo));                
                exportDir.GetFiles("*.hbm.xml")
                    .Length.Should().BePositive("there should be mapping files");
            });
        }


        
        [Scenario]// Warning! this will recreate all db tables
        [InlineData(new object[] { MsSqlAppSettingsCnnKey })]
        public virtual void CreateMsSqlQRTZtables(string appSettingsCnnKey)
        {
            string cmdText = null;
            var expected = -1;
            
            "Given the script to create the table".Given(() =>
            {//arrange
                cmdText = Resources.GetEmbeddedText(typeof(QuartzDbAutomap), 
                    "Zen.QZ.Database.tables_sqlServer.sql");
            });

            "When the command is issued".When(() =>
            {//act
                using (var cmd = new SqlCommand(cmdText,
                        new SqlConnection(ConfigurationManager.AppSettings[appSettingsCnnKey])))
                {
                    cmd.Connection.Open();
                    expected = cmd.ExecuteNonQuery();
                }
            });
                                          
            "Then the script ran".Then(() => Assert.Equal(-1, expected));
        }


        [Scenario]// Warning! this will recreate all db tables
        [InlineData(new object[] { SqLiteDbName })]
        public virtual void CreateSqlLiteQRTZtables(string dbName)
        {
            string cmdText = null;
            var expected = 0;
            
            "Given the script to create the table".Given(() =>
            {//arrange
                cmdText = Resources.GetEmbeddedText(typeof(QuartzDbAutomap), 
                    "Zen.QZ.Database.tables_sqlite.sql");
            });

            "When the command is issued".When(() =>
            {//act
                using (var cmd = new SQLiteCommand(cmdText,
                        new SQLiteConnection("URI=file:" + dbName)))
                {
                    cmd.Connection.Open();
                    expected = cmd.ExecuteNonQuery();
                }
            });

            "Then the script ran".Then(() => Assert.Equal(0, expected));
        }


    }
    
}
