using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

namespace System
{
    /// <summary>
    ///   Set of 3 formatting string that, at runtime, represent a method and its
    ///   parameters.
    /// </summary>
    [Serializable]
    internal class MethodFormatStrings
    {
        private readonly string _methodFormat;
        private readonly bool _methodIsGeneric;
        private readonly string _typeFormat;
        private readonly bool _typeIsGeneric;
		
        /// <summary>
        ///   Initializes a new <see cref = "MethodFormatStrings" />.
        /// </summary>
        /// <param name = "typeFormat">
        ///   The formatting string representing the type
        ///   where each generic type argument is represented as a
        ///   formatting argument (e.g. <c>Dictionary&lt;{0},P1}&gt;</c>.
        /// </param>
        /// <param name = "methodFormat">
        ///   The formatting string representing the method (but not the declaring type).
        ///   where each generic method argument is represented as a
        ///   formatting argument (e.g. <c>ToArray&lt;{0}&gt;</c>. 
        /// </param>
        /// <param name = "methodIsGeneric">Indicates whether the method is generic.</param>
        /// <param name = "typeIsGeneric">Indicates whether the type declaring the method is generic.</param>
        internal MethodFormatStrings(string typeFormat, bool typeIsGeneric,
                                     string methodFormat,
                                     bool methodIsGeneric)
        {
            this._typeFormat = typeFormat;
            this._methodFormat = methodFormat;
            this._typeIsGeneric = typeIsGeneric;
            this._methodIsGeneric = methodIsGeneric;
        }
		
		
        /// <summary>
        ///   Gets a string representing a concrete method invocation.
        /// </summary>
        /// <param name = "invocationParameters">Concrete invocation parameters.</param>
        /// <param name="parametersIndexesNotSerializable"></param>
        /// <returns>A representation of the method invocation.</returns>
        public static string FormatWithSerialization(IEnumerable<object> invocationParameters, IEnumerable<int> parametersIndexesNotSerializable)
        {
            var parts = new string[]
                            {
                                SerializeParameters(invocationParameters, parametersIndexesNotSerializable)
                            };
			
            return string.Concat(parts);
        }
		
        private static string SerializeParameters(IEnumerable<object> invocationParameters, IEnumerable<int> parametersIndexesNotSerializable)
        {
            //var logger = Module.RootKernel.Get<ILog>();
            var result = new StringBuilder();
            var formatter = new BinaryFormatter();
            int index = 0;
            foreach (var invocationParameter in invocationParameters)
            {
                // skippa se non serializzabile
                var currentIndex = index++;
                if (parametersIndexesNotSerializable.Any(i => i == currentIndex))
                {
                    continue;
                }
                var parameterSerialized = "";
                if (invocationParameter != null)
                {
                    var memory = new MemoryStream();
                    formatter.Serialize(memory, invocationParameter);
                    var data = memory.ToArray();
                    //logger.DebugFormat("[Cache] param serialized length: {0}", data.Length);
                    //string parameterSerialized = System.Text.Encoding.Default.GetString(data);
                    //parameterSerialized = Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(parameterSerialized));
                    parameterSerialized = Convert.ToBase64String(data);
                }
                //logger.DebugFormat("[Cache] param serialized: {0}", parameterSerialized);
                result.Append("[");
                var parameterSerializedHash = parameterSerialized.GetHashCode();
                result.Append(parameterSerializedHash);
                //logger.DebugFormat("[Cache] param serialized hashed: {0}", parameterSerializedHash);
                result.Append("]");
            }
            return result.ToString();
        }
		
        internal string FormatPrefix(MethodBase method)
        {
            var parts = new[]
                            {
                                _typeIsGeneric
                                    ? Formatter.FormatString(_typeFormat, method.DeclaringType.GetGenericArguments())
                                    : _typeFormat,
                                _methodIsGeneric
                                    ? Formatter.FormatString(_methodFormat, method.GetGenericArguments())
                                    : _methodFormat,
                            };
			
            return string.Concat(parts);
        }
    }

    /// <summary>
    ///   Provides formatting string representing types, methods and fields. The
    ///   formatting strings may contain arguments like <c>{0}</c> 
    ///   filled at runtime with generic parameters or method arguments.
    /// </summary>
    internal static class Formatter
    {
        /// <summary>
        ///   Gets a formatting string representing a <see cref = "Type{TType}" />.
        /// </summary>
        /// <param name = "type">A <see cref = "Type{TType}" />.</param>
        /// <returns>A formatting string representing the type
        ///   where each generic type argument is represented as a
        ///   formatting argument (e.g. <c>Dictionary&lt;{0},P1}&gt;</c>.
        /// </returns>
        private static string GettypeFormatString(Type type)
        {
            var stringBuilder = new StringBuilder();
			
            // Build the format string for the declaring type.
			
            stringBuilder.Append(type.FullName);
			
            if (type.IsGenericTypeDefinition)
            {
                stringBuilder.Append("<");
                for (var i = 0; i < type.GetGenericArguments().Length; i++)
                {
                    if (i > 0)
                        stringBuilder.Append(", ");
                    stringBuilder.AppendFormat("{{{0}}}", i);
                }
                stringBuilder.Append(">");
            }
            return stringBuilder.ToString();
        }
		
        /// <summary>
        ///   Gets the formatting strings representing a method.
        /// </summary>
        /// <param name = "method">A <see cref = "MethodBase" />.</param>
        /// <returns></returns>
        public static MethodFormatStrings GetMethodFormatStrings(MethodBase method)
        {
            bool methodIsGeneric;
			
            var stringBuilder = new StringBuilder();
			
            string typeFormat = GettypeFormatString(method.DeclaringType);
            bool typeIsGeneric = method.DeclaringType.IsGenericTypeDefinition;
			
            // Build the format string for the method name.
            stringBuilder.Length = 0;
            stringBuilder.Append("::");
            stringBuilder.Append(method.Name);
            if (method.IsGenericMethodDefinition)
            {
                methodIsGeneric = true;
                stringBuilder.Append("<");
                for (var i = 0; i < method.GetGenericArguments().Length; i++)
                {
                    if (i > 0)
                        stringBuilder.Append(", ");
                    stringBuilder.AppendFormat("{{{0}}}", i);
                }
                stringBuilder.Append(">");
            }
            else
            {
                methodIsGeneric = false;
            }
            string methodFormat = stringBuilder.ToString();
			
            // Build the format string for parameters.
            stringBuilder.Length = 0;
            var parameters = method.GetParameters();
            stringBuilder.Append("(");
            for (var i = 0; i < parameters.Length; i++)
            {
                if (i > 0)
                {
                    stringBuilder.Append(", ");
                }
                stringBuilder.Append("{{{");
                stringBuilder.Append(i);
                stringBuilder.Append("}}}");
            }
            stringBuilder.Append(")");
			
            return new MethodFormatStrings(typeFormat, typeIsGeneric, methodFormat, methodIsGeneric);
        }
		
        public static string FormatString(string format, params object[] args)
        {
            return args == null ? format : string.Format(format, args);
        }
    }
}