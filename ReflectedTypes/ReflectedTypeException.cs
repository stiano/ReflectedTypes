using System;
using System.Linq;
using System.Runtime.Serialization;

namespace ReflectedTypes
{
    [Serializable]
    public class ReflectedTypeException : Exception
    {
        public ReflectedTypeException()
        {
        }

        public ReflectedTypeException(string message) : base(message)
        {
        }

        public ReflectedTypeException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ReflectedTypeException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        internal static ReflectedTypeException FieldNotFound(string memberType, string typeName, string propertyName)
        {
            return new ReflectedTypeException($"Property '{propertyName}' was not found on '{typeName}'.");
        }
        
        internal static ReflectedTypeException PropertyIsReadonly(string typeName, string propertyName)
        {
            return new ReflectedTypeException($"Property '{propertyName}' on '{typeName}' is read only.");
        }

        public static ReflectedTypeException PropertyIsWriteOnly(string typeName, string propertyName)
        {
            return new ReflectedTypeException($"Property '{propertyName}' on '{typeName}' is write only.");
        }

        internal static ReflectedTypeException MethodNotFound(string typeName, string methodName, Type[] args)
        {
            if (args.Length == 0)
            {
                return new ReflectedTypeException($"Method {methodName} with zero args was not found on '{typeName}'.");
            }
            else
            {
                var parameters = string.Join(", ", args.Select(t => t.Name));
                return new ReflectedTypeException($"Method {methodName} with args ({parameters}) was not found on '{typeName}'.");
            }
        }

        public static Exception MissingIDisposable(string typeName, string proxyTypeName)
        {
            return new ReflectedTypeException($"Type '{typeName}' implements {nameof(IDisposable)} interface, and so must '{proxyTypeName}'.");
        }
    }
}