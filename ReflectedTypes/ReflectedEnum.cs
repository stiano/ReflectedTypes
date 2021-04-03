using System;

namespace ReflectedTypes
{
    /// <summary>
    /// Use to implement an enum to use with a <see cref="ReflectedType"/>
    /// </summary>
    public abstract class ReflectedEnum : IReflectedInstance
    {
        private readonly Type type;

        protected ReflectedEnum(string enumName, int value = 0)
        {
            type = Type.GetType(enumName, true)!;
            SetValue(value);
        }

        public void SetValue(int value)
        {
            Instance = Enum.ToObject(type, value);
        }
        
        public void SetValue(Enum value)
        {
            Instance = Enum.ToObject(type, value);
        }
        
        public void SetValue(byte value)
        {
            Instance = Enum.ToObject(type, value);
        }
        
        public void SetValue(sbyte value)
        {
            Instance = Enum.ToObject(type, value);
        }
        
        public void SetValue(long value)
        {
            Instance = Enum.ToObject(type, value);
        }
        
        public void SetValue(string value)
        {
            Instance = Enum.Parse(type, value);
        }

        public object? Instance { get; private set; }
    }
}