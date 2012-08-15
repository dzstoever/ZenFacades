using System;
using System.Windows;
using Zen.Log;

namespace Zen.Ux.WpfApp
{

    public partial class App : Application
    {
        public App()
        {  //log messages for these application events, assuming logging has been successfully configured
            
            DispatcherUnhandledException += (s,e) => 
                "Unhandled Error!{0}{1}".LogMe(LogLevel.Fatal, Environment.NewLine, e.Exception.FullMessage());

            Exit += (s, e) =>
                "Application Exit.".LogMe(LogLevel.Debug);
        }

    }

}
