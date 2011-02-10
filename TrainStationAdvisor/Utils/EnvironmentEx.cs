using System;
using System.IO;
using System.Reflection;

namespace TrainStationAdvisor.ClassLibrary
{
    public static class EnvironmentEx
    {
        public static string GetAssemblyPath(Assembly assembly)
        {
            return assembly.GetName().CodeBase;
        }

        public static string GetAssemblyDirectory(Assembly assembly)
        {
            return Path.GetDirectoryName(GetAssemblyPath(assembly));
        }

        public static string CallingAssemblyPath
        {
            get
            {
                return GetAssemblyPath(Assembly.GetCallingAssembly());
            }
        }

        public static string CallingAssemblyDirectory
        {
            get
            {
                return GetAssemblyDirectory(Assembly.GetCallingAssembly());
            }
        }
    }
}