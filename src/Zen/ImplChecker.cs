using System;
using System.IO;

namespace Zen
{
    //Todo: add a dll version check 
    public class ImplChecker 
    {
        public virtual bool CheckForDll(string dllFileName)
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var searchPath = AppDomain.CurrentDomain.RelativeSearchPath;
            var binPath = searchPath == null ? baseDir : Path.Combine(baseDir, searchPath);
            var dllPath = Path.Combine(binPath, dllFileName);

            return File.Exists(dllPath);
        }
    }
}