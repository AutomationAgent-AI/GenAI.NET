using System.Collections.Generic;
using System.Linq;

namespace Automation.GenerativeAI
{
    /// <summary>
    /// Describes a type
    /// </summary>
    public class TypeDescriptor
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="type">Type name</param>
        public TypeDescriptor(string type) 
        { 
            Type = type;
        }

        /// <summary>
        /// Type of the parameter, possible values are string, number etc.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// StringType
        /// </summary>
        public static TypeDescriptor StringType => new TypeDescriptor("string");

        /// <summary>
        /// NumberType
        /// </summary>
        public static TypeDescriptor NumberType => new TypeDescriptor("number");

        /// <summary>
        /// IntegerType
        /// </summary>
        public static TypeDescriptor IntegerType => new TypeDescriptor("integer");

        /// <summary>
        /// BooleanType
        /// </summary>
        public static TypeDescriptor BooleanType => new TypeDescriptor("boolean");

        /// <summary>
        /// Converts the type to a dictionary for JSON serialization
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> ToDictionary()
        {
            var properties = new Dictionary<string, object>();
            
            UpdateProperties(properties);

            return properties;
        }

        /// <summary>
        /// Updates properties of type descriptor
        /// </summary>
        /// <param name="properties"></param>
        protected virtual void UpdateProperties(Dictionary<string, object> properties) 
        {
            properties.Add("type", Type);
        }
    }

    /// <summary>
    /// Represents Enum type
    /// </summary>
    public class EnumTypeDescriptor : TypeDescriptor
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public EnumTypeDescriptor(string[] options) : base("string")
        {
            this.Options = options;
        }

        /// <summary>
        /// List of possible values for the parameter if applicable, else it could be null.
        /// </summary>
        public string[] Options { get; set; }

        /// <summary>
        /// Update properties of Enum type
        /// </summary>
        /// <param name="properties"></param>
        protected override void UpdateProperties(Dictionary<string, object> properties)
        {
            base.UpdateProperties(properties);
            properties.Add("enum", Options);
        }
    }

    /// <summary>
    /// Represents an Array type
    /// </summary>
    public class ArrayTypeDescriptor : TypeDescriptor
    {
        /// <summary>
        /// Default contructor
        /// </summary>
        /// <param name="itemType"></param>
        public ArrayTypeDescriptor(TypeDescriptor itemType) : base("array")
        {
            ItemType = itemType;
        }

        /// <summary>
        /// If the type is array, it represents the type of item.
        /// </summary>
        public TypeDescriptor ItemType { get; set; }

        /// <summary>
        /// Updates the properties of Array type
        /// </summary>
        /// <param name="properties"></param>
        protected override void UpdateProperties(Dictionary<string, object> properties)
        {
            base.UpdateProperties(properties);
            var itemprops = ItemType.ToDictionary();
            properties.Add("items", itemprops);
        }
    }

    /// <summary>
    /// Describes an Object Type
    /// </summary>
    public class ObjectTypeDescriptor : TypeDescriptor
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="properties"></param>
        public ObjectTypeDescriptor(IEnumerable<ParameterDescriptor> properties) : base("object")
        {
            Properties = properties.ToList();
        }

        /// <summary>
        /// List of properties/parameters of the object type
        /// </summary>
        public List<ParameterDescriptor> Properties { get; set; }

        /// <summary>
        /// Updates the properties of the object type
        /// </summary>
        /// <param name="properties"></param>
        protected override void UpdateProperties(Dictionary<string, object> properties)
        {
            base.UpdateProperties(properties);
            var required = new List<string>();
            var props = new Dictionary<string, object>();

            foreach (var parameter in Properties)
            {
                if(parameter.Required)
                {
                    required.Add(parameter.Name);
                }

                var prop = parameter.Type.ToDictionary();
                prop.Add("description", parameter.Description);
                props.Add(parameter.Name, prop);
            }

            properties.Add("properties", props);
            properties.Add("required", required);
        }
    }

    /// <summary>
    /// Provides description of a function parameter.
    /// </summary>
    public class ParameterDescriptor
    {
        /// <summary>
        /// Name of the parameter
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the parameter
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Type of the parameter, possible values are string, number etc.
        /// </summary>
        public TypeDescriptor Type { get; set; } = TypeDescriptor.StringType;

        /// <summary>
        /// Flag to check if the parameter is required or optional
        /// </summary>
        public bool Required { get; set; } = true;
    }

    /// <summary>
    /// Provides description of a function
    /// </summary>
    public class FunctionDescriptor
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name">Name of the fucntion</param>
        /// <param name="description">Description of the function</param>
        /// <param name="parameters">List of parameters</param>
        public FunctionDescriptor(string name, string description, IEnumerable<ParameterDescriptor> parameters)
        {
            Name = name;
            Description = description;
            Parameters = new ObjectTypeDescriptor(parameters);
        }

        /// <summary>
        /// Name of the function
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the function
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Parameters details as needed by the function
        /// </summary>
        public ObjectTypeDescriptor Parameters { get; set; }

        /// <summary>
        /// Provides a list of input parameters.
        /// </summary>
        public IEnumerable<string> InputParameters => Parameters.Properties.Select(p => p.Name);
    }
}
