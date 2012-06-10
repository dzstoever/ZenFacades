using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Zen.Log
{    
    
    /// <summary>
    /// Provides a canned log4net configuration without the need for an .xml file
    /// </summary>
    /// <remarks> Settings overrides for Zen.Log.Log4netConfigurator
    /// ["log-level"]         ?? RootLogLevel; 
    /// ["log-pattern"]       ?? DefaultPattern;
    /// ["log-eventlog"]      ?? EventLogName; 
    /// ["log-filepath"]      ?? FilePath;
    /// ["log-sizekb"]        ?? FileMaxSizeKB;
    /// ["log-rollbacks"]     ?? FileMaxRollbacks;
    /// ["log-smtphost"]      ?? SmtpHost;
    /// ["log-emailto"]       ?? SmtpEmailTo;
    /// ["log-emailfrom"]     ?? SmtpEmailFrom;
    /// ["log-emailsubject"]  ?? SmtpEmailSubject;
    /// ["log-sqlproc"]       ?? SqlProcName;
    /// ["log-sqlcnnstring"]  ?? SqlCnnString;
    /// </remarks>
    /// <see cref="http://www.codeproject.com/Articles/140911/log4net-Tutorial"/>
    public static class Log4netConfigurator
    { 
        /// <summary>
        /// Loads default settings for the built in appenders
        /// </summary>
        static Log4netConfigurator()
        {
            RootLogLevel = LogLevel.All;
            DefaultPattern = "%date{yyyy.MM.dd HH:mm:ss} |%-5level| ~ %logger > %message %n";
            EventLogName = "ZenEventLog";
            FilePath = "logfile.zen";
            FileMaxSizeKB = "4096";
            FileMaxRollbacks = "9";                        
            SmtpHost = "mailserv.eisi.local";
            SmtpEmailTo = "daniel.stoever@eisi.local";
            SmtpEmailFrom = StandardLogName + "@zen.log";
            SmtpEmailSubject = StandardLogName + " Error!";
            SqlProcName = @"noc.dbo.usp_insertMessage2";
            SqlCnnString = @"Data Source= localhost\SQLEXPRESS; Integrated Security=True; Pooling=False";
            
            _loggers = new Dictionary<string, IEnumerable<Appenders>>();
                //{ {"Zen", new[] {Appender.Console, Appender.Debug, Appender.Trace}} };
            _loggerLevels = new Dictionary<string, LogLevel>(); 
                //{ {"Zen", LogLevel.All} };

            //create the dictionary of available appenders *by default all are off 
            _appenders = new Dictionary<Appenders, OnOff>
            {
                {Appenders.Console,      OnOff.Off},
                {Appenders.Debug,        OnOff.Off},
                {Appenders.EventLog,     OnOff.Off},
                {Appenders.File,         OnOff.Off},
                {Appenders.Rtb,          OnOff.Off},
                {Appenders.Smtp,         OnOff.Off},
                {Appenders.Sql,          OnOff.Off},
                {Appenders.Trace,          OnOff.Off},
                //{ExternalAppenders.NHibernate,OnOffSwitch.Off},
            };
            //set default Min levels
            _appenderMinLevels = new Dictionary<Appenders, LogLevel>
            {
                {Appenders.Console,      LogLevel.Debug},
                {Appenders.Debug,        LogLevel.Debug},
                {Appenders.EventLog,     LogLevel.Warn},
                {Appenders.File,         LogLevel.Debug},
                {Appenders.Rtb,          LogLevel.Debug},
                {Appenders.Smtp,         LogLevel.Error},
                {Appenders.Sql,          LogLevel.Info},
                {Appenders.Trace,        LogLevel.Debug},
                //{ExternalAppenders.NHibernate,OnOffSwitch.Off},
            };
            //set default Max levels
            _appenderMaxLevels = new Dictionary<Appenders, LogLevel>
            {
                {Appenders.Console,      LogLevel.Fatal},
                {Appenders.Debug,        LogLevel.Fatal},
                {Appenders.EventLog,     LogLevel.Fatal},
                {Appenders.File,         LogLevel.Fatal},
                {Appenders.Rtb,          LogLevel.Fatal},
                {Appenders.Smtp,         LogLevel.Fatal},
                {Appenders.Sql,          LogLevel.Fatal},
                {Appenders.Trace,        LogLevel.Fatal},
                //{ExternalAppenders.NHibernate,OnOffSwitch.Off},
            };

            LoadConfigOverrides();
        }

        /// <summary>
        /// This must be set to override config options
        /// </summary>
        /// <example>
        /// Log4netConfigurator.Settings = 
        ///     System.Configuration.ConfigurationManager.AppSettings;
        /// </example>
        public static NameValueCollection Settings
        { set { _settings = value; }
        }
        private static NameValueCollection _settings;

        #region config options

        /// <summary>
        /// If you want to configure log4net using an xml file, call .Configure() directly.
        /// </summary>
        /// <remarks>don't do this you silly fool</remarks>
        public static string ConfigureFromFile
        {
            get { return null; }
            set { throw new ApplicationException("If you want to configure log4net using an xml file, call .Configure() directly."); }
        }

        /// <summary>Root Log level priority,
        /// setting this to 'Off' will stop unwanted output 
        /// from all external loggers, like NHibernate/Quartz
        /// </summary>
        public static LogLevel RootLogLevel { get; set; }

        /// <summary>Messages from our appenders will look like this        
        /// <para>
        /// .EventLog and .Smtp use log4net.Layout.ExceptionLayout
        /// .Sql passes the message directly to the stored procedure
        /// </para>
        /// </summary>
        public static string DefaultPattern { get; set; }

        /// <summary>Set this if you want to handle logging errors
        /// </summary>
        public static Type ErrorHandler { get; set; }

        /// <summary>Logging errors to be handled by these appenders (Smtp and Sql excluded)
        /// For example, If set to Appenders.File, any logging errors that occur when logging to 
        /// the other Appenders would be reported in the local file log. 
        /// <para>
        /// This should be set to the most reliable appender(usually Appenders.File)
        /// </para>
        /// </summary>
        public static IEnumerable<Appenders> ErrorHandlerAppenders { get; set; }

        /// <summary>Name of the 'Event Log' in Event Viewer
        /// </summary>
        public static string EventLogName { get; set; }

        /// <summary>Absolute or relative path for the File appender
        /// </summary>       
        public static string FilePath { get; set; }
        
        /// <summary>Maximum size for a local log file
        /// </summary>       
        public static string FileMaxSizeKB { get; set; }
        
        /// <summary>Maximum number of rollbacks to keep
        /// </summary>       
        public static string FileMaxRollbacks { get; set; }

        /// <summary>Sends messages via Email 
        /// </summary>
        public static string SmtpHost { get; set; }
        
        /// <summary>Format must be x@x.xxx, y@y.yyy,...(Comma-delimited)
        /// </summary>
        public static string SmtpEmailTo { get; set; }
        
        /// <summary>Format must be x@x.xxx
        /// </summary>        
        public static string SmtpEmailFrom { get; set; }
        
        /// <summary>Subject of the email
        /// <para>SmtpHost sends Fatal level messages via Email</para>
        /// </summary>
        public static string SmtpEmailSubject { get; set; }

        /// <summary>Sends messages to a stored procedure
        /// This property can not be set, use SqlCnnString & SqlCmdText
        /// </summary>
        public static string SqlDataSource
        {
            get
            {
                var sqlCnnBuilder = new SqlConnectionStringBuilder(SqlCnnString);
                return sqlCnnBuilder.DataSource;
            }
        }
        
        /// <summary>The connection to the database
        /// </summary>
        public static string SqlCnnString { get; set; }
        
        /// <summary>The stored procedure name
        /// </summary>
        public static string SqlProcName { get; set; }

        #endregion


        /// <summary>Uses the process executable name, or "Zen"
        /// </summary>
        public static string StandardLogName
        {
            get
            {
                var a =  Assembly.GetEntryAssembly();                
                return a == null ? "Zen" : a.GetName().Name;

                /* 
                try
                { return Assembly.GetEntryAssembly().GetName().Name; }
                catch (NullReferenceException)
                { }*/
            }
        }


        /// <summary>The current log4net configuration as an xml string
        /// <para>
        /// Purpose is informational, this could be copied
        /// and used as as starting point for custom config
        /// </para>
        /// </summary>
        public static string Log4NetXml { get; private set; }


        /// <summary>Builds the root logger element and a logger element for each logger 
        /// in the _loggers collection, programmatically
        /// <para>
        /// The heirarchy follows a patttern similiar to namespaces,
        /// so assigning appenders to a logger named Zen for examples will
        /// include loggers named Zen.*, Zen.Core.*, Zen.Core.Entities.*, etc.
        /// </para>
        /// </summary>
        private static string Log4NetHeirarchyXml
        {
            get
            {
                var sb = new StringBuilder("<log4net>" + Environment.NewLine);
                
                sb.AppendLine("<root>");
                sb.AppendLine(string.Format("<level value='{0}'/>", RootLogLevel));
                
                //send All messages to the these appenders, if on
                if (_appenders[Appenders.Trace] == OnOff.On) sb.AppendLine("<appender-ref ref='Trace'/>");
                if (_appenders[Appenders.Debug] == OnOff.On) sb.AppendLine("<appender-ref ref='Debug'/>");                
                if (_appenders[Appenders.Console] == OnOff.On) sb.AppendLine("<appender-ref ref='Console'/>"); 
                
                sb.AppendLine("</root>" + Environment.NewLine);
                
                foreach (var loggerName in _loggers.Keys)
                {
                    sb.AppendLine(string.Format("<logger name='{0}'>", loggerName));
                    sb.AppendLine(string.Format("<level value='{0}'/>", _loggerLevels[loggerName]));
                    var appenders = _loggers[loggerName];
                    if(appenders != null)
                    {
                        foreach (var appender in appenders)
                        {
                            switch (appender)
                            {
                                //case Appender.Trace: if (_appenders[Appender.Trace] == OnOff.On) sb.AppendLine("<appender-ref ref='Trace'/>"); break;
                                //case Appender.Debug: if (_appenders[Appender.Debug] == OnOff.On) sb.AppendLine("<appender-ref ref='Debug'/>"); break;
                                //case Appender.Console: if (_appenders[Appender.Console] == OnOff.On) sb.AppendLine("<appender-ref ref='Console'/>"); break;                            
                                case Appenders.EventLog: if (_appenders[Appenders.EventLog] == OnOff.On) sb.AppendLine("<appender-ref ref='EventLog'/>"); break;
                                case Appenders.File: if (_appenders[Appenders.File] == OnOff.On) sb.AppendLine("<appender-ref ref='File'/>"); break;
                                case Appenders.Rtb: if (_appenders[Appenders.Rtb] == OnOff.On) sb.AppendLine("<appender-ref ref='Rtb'/>"); break;
                                case Appenders.Smtp: if (_appenders[Appenders.Smtp] == OnOff.On) sb.AppendLine("<appender-ref ref='Smtp'/>"); break;
                                case Appenders.Sql: if (_appenders[Appenders.Sql] == OnOff.On) sb.AppendLine("<appender-ref ref='Sql'/>"); break;
                                
                            }
                        }
                    }
                    sb.AppendLine("</logger>" + Environment.NewLine);
                }

                if (ErrorHandlerAppenders == null) return sb.ToString();

                //send Logging error messages to these appenders, if On
                sb.AppendLine("<logger name='Log4netErrorHandler'>");
                sb.AppendLine("<level value='Error'/>");
                foreach (var appender in ErrorHandlerAppenders)
                {
                    switch (appender)
                    {
                        //case Appender.Trace: if (_appenders[Appender.Trace] == OnOff.On) sb.AppendLine("<appender-ref ref='Trace'/>"); break;
                        //case Appender.Debug: if (_appenders[Appender.Debug] == OnOff.On) sb.AppendLine("<appender-ref ref='Debug'/>"); break;                        
                        //case Appender.Console: if (_appenders[Appender.Console] == OnOff.On) sb.AppendLine("<appender-ref ref='Console'/>"); break;
                        case Appenders.EventLog: if (_appenders[Appenders.EventLog] == OnOff.On) sb.AppendLine("<appender-ref ref='EventLog'/>"); break;
                        case Appenders.File: if (_appenders[Appenders.File] == OnOff.On) sb.AppendLine("<appender-ref ref='File'/>"); break;
                        case Appenders.Rtb: if (_appenders[Appenders.Rtb] == OnOff.On) sb.AppendLine("<appender-ref ref='Rtb'/>"); break;
                        //case Appender.Smtp: if (_appenders[Appender.Smtp] == OnOff.On) sb.AppendLine("<appender-ref ref='Smtp'/>"); break;
                        //case Appender.Sql: if (_appenders[Appender.Sql] == OnOff.On) sb.AppendLine("<appender-ref ref='Sql'/>"); break;
                    }
                }
                sb.AppendLine("</logger>" + Environment.NewLine);

                return sb.ToString();
            }
        }
        
        /// <summary>Dictionary where the value indicates which appenders are 
        /// used for each logger (key= logger name)  
        /// </summary>
        /// <remarks>Don't add the 'Debug' appender, it's used for all loggers (root level)</remarks>
        private static readonly IDictionary<string, IEnumerable<Appenders>> _loggers;
        
        /// <summary>Min level-value filters applied to each logger        
        /// </summary>
        private static readonly IDictionary<string, LogLevel> _loggerLevels;

        /// <summary>Dictionary where the value indicates whether the appender 
        /// is On(true) or Off(false) - applied to all loggers
        /// </summary>        
        private static readonly IDictionary<Appenders, OnOff> _appenders;
        
        /// <summary>Min level filters applied to each appender, 
        /// except .Smtp which only reports on Error/Fatal
        /// </summary>
        private static readonly IDictionary<Appenders, LogLevel> _appenderMinLevels;
        
        /// <summary>Max level filters applied to each appender, 
        /// except .Smtp which only reports on Error/Fatal
        /// </summary>
        private static readonly IDictionary<Appenders, LogLevel> _appenderMaxLevels;

        
        /// <summary>Adds or changes the min level, and list of appenders used for the given logger,
        /// configure=true to apply the configuration change immediately
        /// </summary>        
        public static void SetLoggerAppenders(string loggerName, LogLevel minLevel, IEnumerable<Appenders> appenders)
        {
            if (!_loggers.ContainsKey(loggerName)) _loggers.Add(loggerName, appenders);
            else _loggers[loggerName] = appenders;

            if (!_loggerLevels.ContainsKey(loggerName)) _loggerLevels.Add(loggerName, minLevel);
            else _loggerLevels[loggerName] = minLevel;
        }
        public static void SetLoggerAppenders(string loggerName, LogLevel minLevel, IEnumerable<Appenders> appenders, bool configure)
        {
            SetLoggerAppenders(loggerName, minLevel, appenders);
            if (configure) Configure();
        }
        
        /// <summary>Turn all appenders on/off programmatically, 
        /// configure=true to apply the configuration change immediately
        /// </summary>        
        public static void TurnAllAppenders(OnOff onoff)
        {
            var appenders = new Appenders[_appenders.Count];
            _appenders.Keys.CopyTo(appenders, 0);//must copy to change values during iteration
            
            foreach (var appender in appenders) TurnAppender(appender, onoff);
        }
        public static void TurnAllAppenders(OnOff onoff, bool configure)
        {
            TurnAllAppenders(onoff);
            if (configure) Configure();
        }

        /// <summary>Turn a collection of appenders on/off programmatically
        /// configure=true to apply the configuration change immediately
        /// </summary>
        public static void TurnAppenders(IEnumerable<Appenders> appenders, OnOff onoff)
        {
            foreach (var appender in appenders) TurnAppender(appender, onoff);            
        }
        public static void TurnAppenders(IEnumerable<Appenders> appenders, OnOff onoff, bool configure)
        {
            TurnAppenders(appenders, onoff);
            if (configure) Configure();
        }
        
        /// <summary>Turn a single appender on/off programmatically
        /// configure=true to apply the configuration change immediately
        /// </summary>
        public static void TurnAppender(Appenders appender, OnOff onoff)
        {
            //turn of the RtbAppender Off if the the type is not available
            if (appender == Appenders.Rtb && onoff == OnOff.On
                && Type.GetType("Zen.Log.Appenders.RtbAppender, Zen") == null) return;
                            
            _appenders[appender] = onoff;            
        }
        public static void TurnAppender(Appenders appender, OnOff onoff, bool configure)
        {
            TurnAppender(appender, onoff);
            if (configure) Configure();
        }

        /// <summary>Turns off all logging for the given logger, excluding 'Debug',
        /// configure=true to apply the configuration change immediately
        /// <para>
        /// To turn off Debug messages, use AppendersSwith(Appender.Debug, OnOff.Off, true),
        /// </para>
        /// </summary>
        public static void TurnLoggerOff(string loggerName)
        {
            if (!_loggers.ContainsKey(loggerName)) _loggers.Add(loggerName, null);
            if (!_loggerLevels.ContainsKey(loggerName)) _loggerLevels.Add(loggerName, LogLevel.Off);
        }
        public static void TurnLoggerOff(string loggerName, bool configure)
        {
            TurnLoggerOff(loggerName);
            if (configure) Configure();
        }


        /// <summary>Builds a canned XmlDocument from embedded xml appenders and calls:
        /// log4net.Config.XmlConfigurator.Configure()
        /// <para>
        /// This can be called at any time to reconfigure logging
        /// </para>
        /// </summary>
        public static void Configure()
        {
            //load xml from our built in appenders/config
            var xmlDoc = new XmlDocument();            
            Log4NetXml = AddAppenders();                
            xmlDoc.LoadXml(Log4NetXml);
            //System.Diagnostics.Trace.Write(Log4NetXml);

            var xmlConfiguratorType = Type.GetType("log4net.Config.XmlConfigurator, log4net");
            if (xmlConfiguratorType == null)
                throw new DependencyException("log4net.dll could not be loaded.");

            //calling log4net.Config.XmlConfigurator.Configure(xmlDoc.DocumentElement) using reflection
            var configureMethodInfo = xmlConfiguratorType.GetMethod("Configure",
                BindingFlags.Static | BindingFlags.Public, Type.DefaultBinder, new[] { typeof(XmlElement) }, null);
            configureMethodInfo.Invoke(null, new object[] { xmlDoc.DocumentElement });
        }


        //use for testing purposes
        public static void LogAllLevelMessages(ILogger logger)
        {
            logger.Debug("Hey, are you a programmer? Logger.Name = " + logger.Name);
            logger.Info("Here's something interesting.");
            logger.Warn("Uh-oh, that's disturbing.");
            logger.Error("That was unexpected.");
            logger.Fatal("The roof is on fire!");
        }


        /// <summary>
        /// Some default settings can be overriden in app.config
        /// </summary>
        private static void LoadConfigOverrides()
        {
            if (_settings == null) return;

            var rootLogLevel = _settings["log-level"] ?? RootLogLevel.ToString();
            LogLevel logLevel;
            if(Enum.TryParse(rootLogLevel, true, out logLevel)) 
                RootLogLevel = (LogLevel)Enum.Parse(logLevel.GetType(), rootLogLevel, true);   
         
            DefaultPattern =    _settings["log-pattern"]         ?? DefaultPattern;
            EventLogName =      _settings["log-eventlog"]        ?? EventLogName;            
            FilePath =          _settings["log-filepath"]        ?? FilePath;
            FileMaxSizeKB =     _settings["log-sizekb"]          ?? FileMaxSizeKB;
            FileMaxRollbacks =  _settings["log-rollbacks"]       ?? FileMaxRollbacks; 
            SmtpHost =          _settings["log-smtphost"]        ?? SmtpHost;
            SmtpEmailTo =       _settings["log-emailto"]         ?? SmtpEmailTo;
            SmtpEmailFrom =     _settings["log-emailfrom"]       ?? SmtpEmailFrom;
            SmtpEmailSubject =  _settings["log-emailsubject"]    ?? SmtpEmailSubject;
            SqlProcName =       _settings["log-sqlproc"]         ?? SqlProcName;
            SqlCnnString =      _settings["log-sqlcnnstring"]    ?? SqlCnnString;
        }
       
        /// <summary>
        /// builds xml from embedded appender configurations
        /// </summary>        
        private static string AddAppenders()
        {
            var sbXml = new StringBuilder(Log4NetHeirarchyXml);//start with the root heirarchy

            const string xmlResourcePath = "Zen.Log.Appenders.";
            if (_appenders[Appenders.Console] == OnOff.On) AddAppender(sbXml, xmlResourcePath + "Console.xml");
            if (_appenders[Appenders.Debug] == OnOff.On) AddAppender(sbXml, xmlResourcePath + "Debug.xml");
            if (_appenders[Appenders.EventLog] == OnOff.On) AddAppender(sbXml, xmlResourcePath + "EventLog.xml");
            if (_appenders[Appenders.File] == OnOff.On) AddAppender(sbXml, xmlResourcePath + "File.xml");
            if (_appenders[Appenders.Rtb] == OnOff.On) AddAppender(sbXml, xmlResourcePath + "Rtb.xml");
            if (_appenders[Appenders.Smtp] == OnOff.On) AddAppender(sbXml, xmlResourcePath + "Smtp.xml");
            if (_appenders[Appenders.Sql] == OnOff.On) AddAppender(sbXml, xmlResourcePath + "Sql.xml");
            if (_appenders[Appenders.Trace] == OnOff.On) AddAppender(sbXml, xmlResourcePath + "Trace.xml");
            //if (Appenders[BuiltInAppenders.NHibernate] == OnOffSwitch.On) 

            sbXml.Append("</log4net>"); //close the root xml element
            return sbXml.ToString();
        }
        
        /// <summary>
        /// Extracts the resource files and adds the xml contents to the stringbuilder.
        /// Also inserts settings into the appenders.
        /// </summary>
        private static void AddAppender(StringBuilder sb, string embeddedFileName)
        {
            //sb.AppendLine("");
            var appenderStream = Resources.GetEmbeddedFile(typeof(Log4netConfigurator), embeddedFileName);

            var xmlText = "";
            using (var xmlReader = new XmlTextReader(appenderStream))
                while (xmlReader.Read())
                    xmlText = xmlReader.ReadOuterXml();

            //add error handler if set
            if (ErrorHandler != null && ErrorHandler.AssemblyQualifiedName != null && Type.GetType(ErrorHandler.AssemblyQualifiedName) != null)
                xmlText = xmlText.Replace("</appender>", string.Format(
                    "<errorHandler type='{0}' />" + Environment.NewLine +
                    "</appender>", ErrorHandler.AssemblyQualifiedName));

            //replace min and max level values
            var appenderName = embeddedFileName.Split(new[] { '.' })[3];
            var key = _appenderMinLevels.Keys.FirstOrDefault(appender => appender.ToString() == appenderName);
            xmlText = xmlText.Replace("~LevelMin~", _appenderMinLevels[key].ToString())
                             .Replace("~LevelMax~", _appenderMaxLevels[key].ToString());

            //add the appender text with any dynamic values replaces
            sb.AppendLine
            (xmlText.Replace("~DefaultPattern~", DefaultPattern)
                    .Replace("~EventLogName~", EventLogName)
                    .Replace("~FilePath~", FilePath)
                    .Replace("~FileMaxSize~", FileMaxSizeKB)
                    .Replace("~FileMaxRollbacks~", FileMaxRollbacks)
                    .Replace("~SmtpHost~", SmtpHost)
                    .Replace("~SmtpEmailTo~", SmtpEmailTo)
                    .Replace("~SmtpEmailFrom~", SmtpEmailFrom)
                    .Replace("~SmtpEmailSubject~", SmtpEmailSubject)
                    .Replace("~SqlCnnString~", SqlCnnString)
                    .Replace("~SqlProcName~", SqlProcName)
                    .Replace("~StandardLogName~", StandardLogName)
            );

        }

    }
}