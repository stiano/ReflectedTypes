namespace ReflectedTypes.Tests
{
    public struct MyStruct
    {
        public int Value { get; set; }
    }

    public class MyStructProxy : ReflectedType
    {
        public MyStructProxy(int value) 
            : base($"ReflectedTypes.Tests.{nameof(MyStruct)}, ReflectedTypes.Tests")
        {
            Value = value;
        }

        public int Value
        {
            get => (int) GetProperty("Value");
            set => SetProperty("Value", value);
        }
    }
}