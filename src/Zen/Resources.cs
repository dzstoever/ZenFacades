using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Zen
{
    /// <summary>
    /// Extracts an embedded file out of a given assembly.
    /// </summary>
    /// <example>qualifiedName = RootNamespace.Folder2.Filename.xml</example>
    public static class Resources
    {
        public static Stream GetEmbeddedFile(Type typeFromAssembly, string qualifiedName)
        {
            var assemblyName = typeFromAssembly.Assembly.GetName().Name;
            return GetEmbeddedFile(assemblyName, qualifiedName);
        }

        public static Stream GetEmbeddedFile(Assembly assembly, string qualifiedName)
        {
            var assemblyName = assembly.GetName().Name;
            return GetEmbeddedFile(assemblyName, qualifiedName);
        }

        public static Stream GetEmbeddedFile(string assemblyName, string qualifiedName)
        {
            var assembly = Assembly.Load(assemblyName);
            var stream = assembly.GetManifestResourceStream(qualifiedName);
            if (stream == null)
                throw new Exception(string.Format(
                    "Could not locate embedded resource '{0}' in '{1}'", qualifiedName, assemblyName));
            return stream;

        }

        public static XmlDocument GetEmbeddedXml(Type typeFromAssembly, string qualifiedName)
        {
            var stream = GetEmbeddedFile(typeFromAssembly, qualifiedName);
            var xmlDoc = new XmlDocument();
            using(var xmlReader = new XmlTextReader(stream))
                xmlDoc.Load(xmlReader);
            return xmlDoc;
        }

        public static string GetEmbeddedText(Type typeFromAssembly, string embeddedFileName)
        {
            var stream = GetEmbeddedFile(typeFromAssembly, embeddedFileName);
            string text;
            using (var reader = new StreamReader(stream))
                text = reader.ReadToEnd();
            return text;
        }
    }
}
