using System;
using System.IO;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Instances;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace Zen.Quartz.Automap
{
    /// <summary>Essentially a SessionFactoryFactory, with a method to 
    /// create a persistence model based on FluentNHibernate.Automapping
    /// </summary>
    public class QuartzDbAutomap
    {
        public const string DefaultDbName = "QuartzDb";

        //Todo: move these QuartzDbAutomap 'SessionFactoryFactory's' to the NHibernate assembly?
        /// <summary>
        /// Configure NHibernate to use Microsoft Sql Server 2008. 
        /// </summary>
        /// <remarks>
        /// Line 1: Begin configuration
        ///      2: Configure the database being used (SQLite file db)
        ///      3: Specify what mappings are going to be used (Automappings from the CreateAutomappings method)
        ///      4: Expose the underlying configuration instance to the BuildSchema method,
        ///      5: Finally, build the session factory.         
        /// </remarks>
        /// <param name="appSettingsCnnKey">key name from the 'appSettings' section to use for the db connection </param>
        /// <param name="exportDbSchema">WARNING! this will create/recreate the database</param> 
        /// <param name="exportMappingsTo">export a copy of your mapping files to this location</param>        
        /// <returns>
        /// This method returns an ISessionFactory instance that is
        /// populated with mappings created by Fluent NHibernate.
        /// </returns>        
        public static ISessionFactory GetMsSql2008SessionFactory(string appSettingsCnnKey, bool exportDbSchema, string exportMappingsTo)
        {
            try
            {
                DirectoryInfo exportDir = null;
                if (!string.IsNullOrEmpty(exportMappingsTo))
                {
                    exportDir = exportMappingsTo.Contains(@"\") ? new DirectoryInfo(exportMappingsTo)
                        : new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, exportMappingsTo));
                    //create the mappings directory, if needed
                    if (!exportDir.Exists) exportDir.Create();
                }

                if (exportDbSchema)
                {
                    //rebuild the database & export the mappings
                    if (exportDir != null) return Fluently.Configure()
                                            .Database(MsSqlConfiguration.MsSql2008.ConnectionString(c => c.FromAppSetting(appSettingsCnnKey)))
                                            .Mappings(m => m.AutoMappings.Add(CreateAutomappings).ExportTo(exportMappingsTo))
                                            .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(false, true))
                                            .BuildSessionFactory();

                    //rebuild the database only
                    return Fluently.Configure()
                        .Database(MsSqlConfiguration.MsSql2008.ConnectionString(c => c.FromAppSetting(appSettingsCnnKey)))
                        .Mappings(m => m.AutoMappings.Add(CreateAutomappings))
                        .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(false, true))
                        .BuildSessionFactory();
                }
                //export the mappings only
                if (exportDir != null) return Fluently.Configure()
                                        .Database(MsSqlConfiguration.MsSql2008.ConnectionString(c => c.FromAppSetting(appSettingsCnnKey)))
                                        .Mappings(m => m.AutoMappings.Add(CreateAutomappings).ExportTo(exportMappingsTo))
                                        .BuildSessionFactory();

                //configure only (NO rebuild & No export)
                return Fluently.Configure()
                    .Database(MsSqlConfiguration.MsSql2008.ConnectionString(c => c.FromAppSetting(appSettingsCnnKey)))
                    .Mappings(m => m.AutoMappings.Add(CreateAutomappings))
                    .BuildSessionFactory();
            }
            catch (Exception ex)
            { throw new DataAccessException("Could not configure data access.", ex); }

        }

        /// <summary>
        /// Configure NHibernate to use SQLite.(a fast in-memory db well suited for testing.) 
        /// </summary>
        /// <remarks>
        /// Line 1: Begin configuration
        ///      2: Configure the database being used (SQLite file db)
        ///      3: Specify what mappings are going to be used (Automappings from the CreateAutomappings method)
        ///      4: Expose the underlying configuration instance to the BuildSchema method,
        ///      5: Finally, build the session factory.         
        /// </remarks>
        /// <param name="dbName">Name of the SqlLite db file to use</param>
        /// <param name="exportDbSchema">WARNING! this will create/recreate the database</param>
        /// <param name="exportMappingsTo">export a copy of your mapping files to this location</param>        
        /// <returns>
        /// This method returns an ISessionFactory instance that is
        /// populated with mappings created by Fluent NHibernate.
        /// </returns>
        public static ISessionFactory GetSqLiteSessionFactory(string dbName, bool exportDbSchema, string exportMappingsTo)
        {
            //bool exportMappings = !string.IsNullOrEmpty(exportMappingsTo);
            try
            {
                DirectoryInfo exportDir = null;
                if (!string.IsNullOrEmpty(exportMappingsTo))
                {
                    exportDir = exportMappingsTo.Contains(@"\") ? new DirectoryInfo(exportMappingsTo)
                        : new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, exportMappingsTo));
                    //create the mappings directory, if needed
                    if (!exportDir.Exists) exportDir.Create();
                }

                if (exportDbSchema)
                {
                    if (File.Exists(dbName)) File.Delete(dbName);
                    //rebuild the database & export the mappings
                    if (exportDir != null) return Fluently.Configure()
                                            .Database(SQLiteConfiguration.Standard.UsingFile(dbName).ShowSql())
                                            .Mappings(m => m.AutoMappings.Add(CreateAutomappings).ExportTo(exportMappingsTo))
                                            .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(false, true))
                                            .BuildSessionFactory();

                    //rebuild the database only
                    return Fluently.Configure()
                        .Database(SQLiteConfiguration.Standard.UsingFile(dbName).ShowSql())
                        .Mappings(m => m.AutoMappings.Add(CreateAutomappings))
                        .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(false, true))
                        .BuildSessionFactory();
                }
                //export the mappings only
                if (exportDir != null) return Fluently.Configure()
                                        .Database(SQLiteConfiguration.Standard.UsingFile(dbName).ShowSql())
                                        .Mappings(m => m.AutoMappings.Add(CreateAutomappings).ExportTo(exportMappingsTo))
                                        .BuildSessionFactory();   
                    
                //configure only (NO rebuild & No export)
                return Fluently.Configure()
                    .Database(SQLiteConfiguration.Standard.UsingFile(dbName).ShowSql())
                    .Mappings(m => m.AutoMappings.Add(CreateAutomappings))
                    .BuildSessionFactory(); 
            }
            catch (Exception ex)
            { throw new DataAccessException("Could not configure data access.", ex); }
        }


        //Todo: override CreateAutomappings to accept configuration instance directly, so we don't have to automap every time
        /// <summary>
        /// This is the actual automapping - use AutoMap to start automapping,
        /// then pick one of the static methods to specify what to map, and then either
        /// 1) use the Setup and Where methods to restrict that behaviour, or
        /// 2) (preferably) supply a configuration instance of your definition to control the automapper. 
        /// </summary>
        /// <returns></returns>
        internal static AutoPersistenceModel CreateAutomappings()
        {            
            //(in this case all the classes in the assembly that contains this class)
            var am = AutoMap.AssemblyOf<QuartzDbAutomap>(new QuartzDbAutomapConfig())
                            //.Override<Scheduler>(m => m.IgnoreProperty(x => x.Guid))                            
                            .Conventions.Add<QuartzDbCascadeConvention>()
							.Conventions.Add<SqlTypeConvention>();

            //*I think this is deprecated so I ignored all members named 'Guid' in the AutoMapConfig
            //.ForTypesThatDeriveFrom<DomainEntity>(e => e.IgnoreProperty(x => x.Guid))
            //am.Conventions.Add(convention => { })

            return am;
        }
        
		
		
        
    }
	
	internal class SqlTypeConvention : FluentNHibernate.Conventions.IPropertyConvention
	{
			
		public void Apply(IPropertyInstance instance)
		{
			if(instance.Type == typeof(DateTime))
				instance.CustomSqlType("smalldatetime");
		}
	 
	}	
}