using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Automation.GenerativeAI.Utilities
{
    internal static class ExtensionMethods
    {
        //
        // Summary:
        //     Converts an object into another type, irrespective of whether the conversion
        //     can be done at compile time or not. This can be used to convert generic types
        //     to numeric types during runtime.
        //
        // Parameters:
        //   value:
        //     The value to be converted.
        //
        // Type parameters:
        //   T:
        //     The destination type.
        //
        // Returns:
        //     The result of the conversion.
        public static T To<T>(this object value)
        {
            return (T)value.To(typeof(T));
        }

        //
        // Summary:
        //     Converts an object into another type, irrespective of whether the conversion
        //     can be done at compile time or not. This can be used to convert generic types
        //     to numeric types during runtime.
        //
        // Parameters:
        //   value:
        //     The value to be converted.
        //
        //   type:
        //     The type that the value should be converted to.
        //
        // Returns:
        //     The result of the conversion.
        public static object To(this object value, Type type)
        {
            if (value == null)
            {
                return Convert.ChangeType(null, type);
            }

            if (type.IsInstanceOfType(value))
            {
                return value;
            }

            Type type2 = value.GetType();
            List<MethodInfo> list = new List<MethodInfo>();
            list.AddRange(type2.GetMethods(BindingFlags.Static | BindingFlags.Public));
            list.AddRange(type.GetMethods(BindingFlags.Static | BindingFlags.Public));
            foreach (MethodInfo item in list)
            {
                if (item.IsPublic && item.IsStatic && (item.Name == "op_Implicit" || item.Name == "op_Explicit") && item.ReturnType == type)
                {
                    ParameterInfo[] parameters = item.GetParameters();
                    if (parameters.Length == 1 && parameters[0].ParameterType.IsInstanceOfType(value))
                    {
                        return item.Invoke(null, new object[1] { value });
                    }
                }
            }

            return Convert.ChangeType(value, type);
        }

        public static double CosineDistance(this double[] x, double[] y)
        {
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            for (int i = 0; i < x.Length; i++)
            {
                num += x[i] * y[i];
                num2 += x[i] * x[i];
                num3 += y[i] * y[i];
            }

            double num4 = System.Math.Sqrt(num2) * System.Math.Sqrt(num3);
            if (num != 0.0)
            {
                return 1.0 - num / num4;
            }

            return 1.0;
        }
    }
}
