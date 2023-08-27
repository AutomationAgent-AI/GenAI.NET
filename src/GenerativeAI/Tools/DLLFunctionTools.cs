using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.Tools;
using Automation.GenerativeAI.Utilities;
using PdfSharp.Drawing.BarCodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;

namespace Automation.GenerativeAI
{
    /// <summary>
    /// Implements a FunctionToolSet for a .net dll. Using reflection it
    /// extracts all the public static methods from a given class in the DLL
    /// to provide it as toolset.
    /// </summary>
    public class DLLFunctionTools : IFunctionToolSet
    {
        class Tool : FunctionTool
        {
            MethodInfo method;
            
            public Tool(FunctionDescriptor descriptor, MethodInfo method)
            {
                this.method = method;
                this.descriptor = descriptor;
                //var name = $"{method.DeclaringType.Namespace}.{method.DeclaringType.Name}.{method.Name}";
                this.Name = descriptor.Name;
                this.Description = descriptor.Description;
            }
            
            protected override async Task<Result> ExecuteCoreAsync(ExecutionContext context)
            {
                var retval = new Result() { success = true, output = string.Empty };
                var args = method.GetParameters().Select(p => ParsePrameterValue(p, context)).ToArray();
                try
                {
                    var result = await Task.Run(() => method.Invoke(null, args));
                    retval.output = result;

                    return retval;
                }
                catch (Exception ex)
                {
                    Logger.WriteLog(LogLevel.Warning, LogOps.Exception, ex.Message);
                    retval.success = false;
                    return retval;
                }
            }

            protected override FunctionDescriptor GetDescriptor()
            {
                return this.descriptor;
            }

            private object ParsePrameterValue(ParameterInfo p, ExecutionContext context)
            {
                object value;
                if (!context.TryGetValue(p.Name, out value) || value == null)
                {
                    if (p.IsOptional) return p.DefaultValue;
                    return Create(p.ParameterType);
                }

                if (value is string && p.ParameterType.IsEnum)
                {
                    return System.Enum.Parse(p.ParameterType, (string)value);
                }

                return value;
            }

            private static T Create<T>()
            {
                var type = typeof(T);
                return (T)Create(type);
            }

            private static object Create(Type targetType)
            {
                //string test first - it has no parameterless constructor
                if (Type.GetTypeCode(targetType) == TypeCode.String)
                    return string.Empty;

                return Activator.CreateInstance(targetType);
            }
        }

        private readonly string dllpath;
        private readonly string classname;
        private static Dictionary<string, string> loadedXmlDocumentation = new Dictionary<string, string>();
        private Dictionary<string, Tool> tools = null;

        /// <summary>
        /// Gets the tool with specific name.
        /// </summary>
        /// <param name="name">Name of the tool</param>
        /// <returns>IFunctionTool if exists in the collection, else throws KeyNotFoundException.</returns>
        public IFunctionTool this[string name] 
        { 
            get
            {
                var tool = GetTool(name);
                if (tool == null) throw new KeyNotFoundException(name);

                return tool;
            }
        }

        /// <summary>
        /// Creates DLLFunctionTools object
        /// </summary>
        /// <param name="dllpath">Full path of a .net DLL</param>
        /// <param name="classname">A fully qualified classname, including namespace.</param>
        public DLLFunctionTools(string dllpath, string classname = "") 
        { 
            this.dllpath = dllpath;
            this.classname = classname;
        }

        /// <summary>
        /// Executes a given function with the arguments passed and returns the 
        /// output as a string. For complex objects it returns JSON string.
        /// </summary>
        /// <param name="functionName">Name of the function to execute</param>
        /// <param name="context">Execution context holding arguments of the function</param>
        /// <returns></returns>
        public async Task<string> ExecuteAsync(string functionName, ExecutionContext context)
        {
            IFunctionTool tool = GetTool(functionName);
            if (tool == null) return $"ERROR: Couldn't find a tool with name '{functionName}'";

            return await tool.ExecuteAsync(context);
        }

        private IEnumerable<MethodInfo> GetMethods(Assembly asm, string classname)
        {
            IEnumerable<Type> types;

            if (string.IsNullOrEmpty(classname))
            {
                types = asm.GetExportedTypes();
            }
            else
            {
                var t = asm.GetType(classname);
                types = new Type[] { t };
            }

            var methods = new List<MethodInfo>();
            foreach (var type in types)
            {
                var m = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
                methods.AddRange(m);
            }

            return methods;
        }

        private Dictionary<string, Tool> GetTools()
        {
            if (tools == null)
            {
                tools = new Dictionary<string, Tool>();

                Assembly asm = Assembly.LoadFrom(dllpath);
                var methods = GetMethods(asm, classname);
                LoadXmlDocumentation(asm, dllpath);

                int id = 1;
                foreach (var m in methods)
                {
                    var functionname = $"{m.Name}_{id++}"; //Generate unique names to take care of overloads
                    var documentation = GetDocumentation(m);
                    var description = GetSummary(documentation);
                    var returns = GetReturns(documentation);
                    if (!string.IsNullOrEmpty(returns))
                    {
                        description = $"{description} {returns}";
                    }

                    var parameters = m.GetParameters().Select(
                        p => new ParameterDescriptor()
                        {
                            Name = p.Name,
                            Description = GetDocumentation(p, documentation),
                            Required = !p.IsOptional,
                            Type = GetParameterType(p.ParameterType),
                        }).ToList();

                    var function = new FunctionDescriptor(functionname, description, parameters);
                    var tool = new Tool(function, m);
                    tools.Add(tool.Name, tool);
                }
            }

            return tools;
        }

        /// <summary>
        /// Returns function descriptors for all the public static functions of a class
        /// this toolset wraps.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FunctionDescriptor> GetFunctions()
        {
            var mytools = GetTools();
            return mytools.Values.Select(t => t.Descriptor);
        }

        private ObjectTypeDescriptor GetObjectTypeDescriptor(Type type)
        {
            if(!type.IsClass) return null;

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var parameters = properties.Select(p => new ParameterDescriptor()
            {
                Name = p.Name,
                Description = GetDocumentation(p),
                Required = p.CanWrite,
                Type = GetParameterType(p.PropertyType)
            });

            return new ObjectTypeDescriptor(parameters.ToList());
        }

        bool IsEnumerable(Type type)
        {
            return type.GetInterface(typeof(IEnumerable).FullName) != null;
        }

        Type GetElementType(Type type)
        {
            // Type is Array
            // short-circuit if you expect lots of arrays 
            if (type.IsArray)
                return type.GetElementType();

            // type is IEnumerable<T>;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return type.GetGenericArguments()[0];

            // type implements/extends IEnumerable<T>;
            var enumType = type.GetInterfaces()
                                    .Where(t => t.IsGenericType &&
                                           t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                                    .Select(t => t.GenericTypeArguments[0]).FirstOrDefault();
            return enumType ?? type;
        }

        private TypeDescriptor GetParameterType(Type type)
        {
            switch (type.Name)
            {
                case "Double":
                    return TypeDescriptor.NumberType;
                case "String":
                    return TypeDescriptor.StringType;
                case "Integer":
                case "Int32":
                    return TypeDescriptor.IntegerType;
                case "Boolean":
                    return TypeDescriptor.BooleanType;
                default:
                    break;
            }

            if (type.IsEnum) return new EnumTypeDescriptor(System.Enum.GetNames(type));

            if(IsEnumerable(type)) return new ArrayTypeDescriptor(GetParameterType(GetElementType(type)));

            if (type.IsClass) return GetObjectTypeDescriptor(type);

            return new TypeDescriptor("Unsupported");
        }

        private static void LoadXmlDocumentation(Assembly assembly, string path)
        {
            string directoryPath = Path.GetDirectoryName(path);
            string xmlFilePath = Path.Combine(directoryPath, assembly.GetName().Name + ".xml");
            if (File.Exists(xmlFilePath))
            {
                LoadXmlDocumentation(File.ReadAllText(xmlFilePath));
            }
        }

        private static void LoadXmlDocumentation(string xmlDocumentation)
        {
            using (XmlReader xmlReader = XmlReader.Create(new StringReader(xmlDocumentation)))
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "member")
                    {
                        string raw_name = xmlReader["name"].Split('(')[0];
                        loadedXmlDocumentation[raw_name] = xmlReader.ReadInnerXml().Replace("\n", "").Replace("\r", "");
                    }
                }
            }
        }

        private static string XmlDocumentationKeyHelper(string typeFullNameString, string memberNameString)
        {
            string key = Regex.Replace(
              typeFullNameString, @"\[.*\]",
              string.Empty).Replace('+', '.');
            if (memberNameString != null)
            {
                key += "." + memberNameString;
            }
            return key;
        }

        private static string GetDocumentation(PropertyInfo propertyInfo)
        {
            string key = "P:" + XmlDocumentationKeyHelper(
              propertyInfo.DeclaringType.FullName, propertyInfo.Name);
            loadedXmlDocumentation.TryGetValue(key, out string documentation);
            return documentation;
        }

        private static string GetDocumentation(MethodInfo methodInfo)
        {
            string key = "M:" + XmlDocumentationKeyHelper(
              methodInfo.DeclaringType.FullName, methodInfo.Name);
            loadedXmlDocumentation.TryGetValue(key, out string documentation);
            return documentation;
        }

        private static string GetDocumentation(ParameterInfo parameterInfo, string memberDocumentation)
        {
            if (string.IsNullOrEmpty(memberDocumentation)) return null;

            string regexPattern =
                Regex.Escape(@"<param name=" + "\"" + parameterInfo.Name + "\"" + @">") +
                "(.*?)" +
                Regex.Escape(@"</param>");

            return GetData(regexPattern, memberDocumentation);
        }

        private static string GetSummary(string documentation)
        {
            if (string.IsNullOrEmpty(documentation)) return null;

            string regexPattern =
                Regex.Escape(@"<summary>") +
                "(.*?)" +
                Regex.Escape(@"</summary>");

            return GetData(regexPattern, documentation);
        }

        private static string GetReturns(string documentation)
        {
            if (string.IsNullOrEmpty(documentation)) return null;

            string regexPattern =
                Regex.Escape(@"<returns>") +
                "(.*?)" +
                Regex.Escape(@"</returns>");

            return GetData(regexPattern, documentation);
        }

        private static string GetData(string regexPattern, string documentation)
        {
            Match match = Regex.Match(documentation, regexPattern);
            if (match.Success)
            {
                var data = match.Groups[1].Value;
                return Regex.Replace(data, @"\s{2,}", @" ").Trim();
            }
            return null;
        }

        /// <summary>
        /// Gets a tool for a specific function name.
        /// </summary>
        /// <param name="functionName">Name of the function to search</param>
        /// <returns>IFunctionTool instance for the corresponding function.</returns>
        public IFunctionTool GetTool(string functionName)
        {
            if(tools == null)
            {
                _ = GetTools();
            }

            var matches = new List<Tool>();
            foreach(var pair in tools)
            {
                //Found exact match
                if(pair.Value.Descriptor.Name.Equals(functionName) 
                    || pair.Key.Equals(functionName))
                    return pair.Value;

                if (pair.Key.Contains(functionName))
                {
                    matches.Add(pair.Value);
                }
            }

            return matches.FirstOrDefault();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<IFunctionTool> GetEnumerator()
        {
            var mytools = GetTools();

            return mytools.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            var mytools = GetTools();

            return mytools.Values.GetEnumerator();
        }
    }
}
