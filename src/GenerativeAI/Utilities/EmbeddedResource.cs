using System;
using System.IO;
using System.Reflection;

namespace Automation.GenerativeAI.Utilities
{
    internal static class EmbeddedResource
    {
        internal static string GetrResource(string name)
        {
            var assembly = typeof(EmbeddedResource).GetTypeInfo().Assembly;
            if (assembly == null) { throw new Exception($"Assembly for resource {name} not found!"); }

            using (Stream resource = assembly.GetManifestResourceStream(name))
            {
                if (resource == null) { throw new Exception($"Resource: '{name}' not found!"); }
                using (var reader = new StreamReader(resource))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
