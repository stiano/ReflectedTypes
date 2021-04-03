using System;
using System.Linq;
using System.Reflection;

namespace ReflectedTypes
{
    /// <summary>
    /// Represents a type that is needs to be reflected on upon runtime. Inherit, and proxy method calls appropriately.
    /// </summary>
    public abstract class ReflectedType : IReflectedInstance
    {
        private static readonly ReflectedTypeComparer ReflectedTypeComparer = new ReflectedTypeComparer();

        private readonly string typeName;
        private readonly Type type;

        public object? Instance { get; }

        /// <summary>
        /// Creates a new instance of <see cref="ReflectedType"/>.
        /// </summary>
        /// <param name="typeName">The fully qualified name of the type. E.g.: Namespace.Type, Assembly</param>
        /// <param name="args">Constructor arguments if any</param>
        protected ReflectedType(string typeName, params object[] args)
        {
            this.typeName = typeName;
            type = Type.GetType(typeName, true)!;

            if (type == null)
                throw new ReflectedTypeException("Could not find instance blah blah blah.");

            var isStaticClass = type.IsAbstract;

            if (!isStaticClass)
            {
                if (NeedsToImplementIDisposable(type, GetType())) 
                    throw ReflectedTypeException.MissingIDisposable(typeName, GetType().FullName!);
                
                Instance = Activator.CreateInstance(type, args);
            }
        }

        /// <summary>
        /// Gets the field value
        /// </summary>
        /// <param name="fieldName">Name of field</param>
        /// <param name="bindingFlags">Optional <see cref="BindingFlags"/></param>
        protected object? GetField(string fieldName, BindingFlags? bindingFlags = null)
        {
            var field = bindingFlags.HasValue
                ? type.GetField(fieldName, bindingFlags.Value)
                : type.GetField(fieldName);

            if (field == null)
                throw ReflectedTypeException.FieldNotFound("Field", typeName, fieldName);

            return field.GetValue(Instance);
        }

        /// <summary>
        /// Sets the field value
        /// </summary>
        /// <param name="fieldName">Name of field</param>
        /// <param name="value">Value of field</param>
        /// <param name="bindingFlags">Optional <see cref="BindingFlags"/></param>
        protected void SetField(string fieldName, object value, BindingFlags? bindingFlags = null)
        {
            value = GetValueOrReflectedInstanceValue(value);

            var field = bindingFlags.HasValue 
                ? type.GetField(fieldName, bindingFlags.Value)
                : type.GetField(fieldName);

            if (field == null)
                throw ReflectedTypeException.FieldNotFound("Field", typeName, fieldName);

            field.SetValue(Instance, value);
        }

        /// <summary>
        /// Sets the property value
        /// </summary>
        /// <param name="propertyName">Name of field</param>
        /// <param name="value">Value of field</param>
        /// <param name="bindingFlags">Optional <see cref="BindingFlags"/></param>
        protected void SetProperty(string propertyName, object value, BindingFlags? bindingFlags = null)
        {
            value = GetValueOrReflectedInstanceValue(value);

            var property = bindingFlags.HasValue 
                ? type.GetProperty(propertyName, bindingFlags.Value)
                : type.GetProperty(propertyName);

            if (property == null)
                throw ReflectedTypeException.FieldNotFound("Property", typeName, propertyName);

            if (!property.CanWrite)
                throw ReflectedTypeException.PropertyIsReadonly(typeName, propertyName);

            property.SetValue(Instance, value);
        }

        /// <summary>
        /// Gets the property value
        /// </summary>
        /// <param name="propertyName">Name of property</param>
        /// <param name="bindingFlags">Optional <see cref="BindingFlags"/></param>
        protected object? GetProperty(string propertyName, BindingFlags? bindingFlags = null)
        {
            var property = bindingFlags.HasValue
                ? type.GetProperty(propertyName, bindingFlags.Value)
                : type.GetProperty(propertyName);

            if (property == null)
                throw ReflectedTypeException.FieldNotFound("Property", typeName, propertyName);

            if (!property.CanRead)
                throw ReflectedTypeException.PropertyIsWriteOnly(typeName, propertyName);

            return property.GetValue(Instance);
        }

        /// <summary>
        /// Invokes a method
        /// </summary>
        /// <param name="methodName">Name of the method</param>
        /// <param name="args">Method arguments, if any</param>
        /// <param name="bindingFlags">Optional <see cref="BindingFlags"/></param>
        protected object? Invoke(string methodName, object[]? args = null, BindingFlags? bindingFlags = null)
        {
            args ??= Array.Empty<object>();
            
            args = args.Select(GetValueOrReflectedInstanceValue).ToArray();
            var methodArgTypes = args.Select(x => x.GetType()).ToArray();

            var method = bindingFlags.HasValue
               ? type.GetMethod(methodName, bindingFlags.Value,null, CallingConventions.Any, methodArgTypes, null)
               : type.GetMethod(methodName, methodArgTypes);

            if (method == null)
                throw ReflectedTypeException.MethodNotFound(typeName, methodName, methodArgTypes);

            return method.Invoke(Instance, args);
        }

        /// <summary>
        /// Invokes a generic method with 1 generic argument.
        /// </summary>
        /// <param name="methodName">Name of the method</param>
        /// <param name="args">Method arguments, if any</param>
        /// <param name="bindingFlags">Optional <see cref="BindingFlags"/></param>
        protected object? InvokeGeneric<T>(string methodName, object[]? args = null, BindingFlags? bindingFlags = null)
        {
            return InvokeGeneric(methodName, new[] {typeof(T)}, args, bindingFlags);
        }

        /// <summary>
        /// Invokes a generic method with 2 generic argument.
        /// </summary>
        /// <param name="methodName">Name of the method</param>
        /// <param name="args">Method arguments, if any</param>
        /// <param name="bindingFlags">Optional <see cref="BindingFlags"/></param>
        protected object? InvokeGeneric<T1, T2>(string methodName, object[]? args = null, BindingFlags? bindingFlags = null)
        {
            return InvokeGeneric(methodName, new[] { typeof(T1), typeof(T2) }, args, bindingFlags);
        }

        /// <summary>
        /// Invokes a generic method with 3 generic argument.
        /// </summary>
        /// <param name="methodName">Name of the method</param>
        /// <param name="args">Method arguments, if any</param>
        /// <param name="bindingFlags">Optional <see cref="BindingFlags"/></param>
        protected object? InvokeGeneric<T1, T2, T3>(string methodName, object[]? args = null, BindingFlags? bindingFlags = null)
        {
            return InvokeGeneric(methodName, new[] { typeof(T1), typeof(T2), typeof(T3) }, args, bindingFlags);
        }

        /// <summary>
        /// Invokes a generic method with x generic argument. Internal usage.
        /// </summary>
        /// <param name="methodName">Name of the method</param>
        /// <param name="genericArgument">A list of all generic argument types</param>
        /// <param name="args">Method arguments, if any</param>
        /// <param name="bindingFlags">Optional <see cref="BindingFlags"/></param>
        protected object? InvokeGeneric(string methodName, Type[] genericArgument, object[]? args = null, BindingFlags? bindingFlags = null)
        {
            args ??= Array.Empty<object>();
            
            args = args.Select(GetValueOrReflectedInstanceValue).ToArray();
            var methodArgTypes = args.Select(x => x.GetType()).ToArray();

            var methods = bindingFlags.HasValue
                ? type.GetMethods(bindingFlags.Value)
                : type.GetMethods();

            var method =  methods
                .Where(m =>
                {
                    if (m.Name != methodName)
                        return false;

                    var parameters = m.GetParameters()
                        .Select(p => p.ParameterType)
                        .ToArray();

                    var matchParameters = parameters.SequenceEqual(methodArgTypes, ReflectedTypeComparer);
                    
                    return matchParameters;
                })
                .FirstOrDefault(x => x.ContainsGenericParameters);

            if (method == null)
                throw ReflectedTypeException.MethodNotFound(typeName, methodName, methodArgTypes);

            method = method.MakeGenericMethod(genericArgument);
            return method.Invoke(Instance, args);
        }

        /// <summary>
        /// If <see cref="IDisposable"/> is implemented, disposes inner object instance, else noop.
        /// </summary>
        public void Dispose()
        {
            if (Instance is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        private static bool NeedsToImplementIDisposable(Type type, Type reflectedType)
        {
            return typeof(IDisposable).IsAssignableFrom(type) &&
                   !typeof(IDisposable).IsAssignableFrom(reflectedType);
        }

        private static object GetValueOrReflectedInstanceValue(object value)
        {
            if (value is IReflectedInstance reflectedInstance)
            {
                return reflectedInstance.Instance!;
            }

            return value;
        }
    }
}
