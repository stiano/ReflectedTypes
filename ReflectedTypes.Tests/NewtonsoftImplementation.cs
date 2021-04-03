using System;
using System.IO;
using System.Text;

namespace ReflectedTypes.Tests
{
    public sealed class FormattingEnum : ReflectedEnum
    {
        public FormattingEnum(Options value) : 
            base("Newtonsoft.Json.Formatting, Newtonsoft.Json")
        {
            SetValue(value);
        }

        public enum Options
        {
            None = 0,
            Indented = 1,            
        }
    }

    public sealed class JsonConvert : ReflectedType
    {
        public JsonConvert() :
            base("Newtonsoft.Json.JsonConvert, Newtonsoft.Json")
        {
        }

        public string SerializeObject(object value, object formatter)
        {
            return (string) Invoke("SerializeObject", new []{ value, formatter });
        }
        
        public T DeserializeObject<T>(string value)
        {
            return (T) InvokeGeneric<T>("DeserializeObject", new object[]{ value });
        }
    }

    public sealed class JsonSerializer : ReflectedType
    {
        public JsonSerializer() :
            base("Newtonsoft.Json.JsonSerializer, Newtonsoft.Json")
        {
        }

        public object ContractResolver
        {
            get => GetProperty("ContractResolver");
            set => SetProperty("ContractResolver", value);
        }

        private void InvokeSerialize(object jsonWriter, object value)
        {
            Invoke("Serialize", new []{ jsonWriter, value });
        }

        private T InvokeDeserialize<T>(object value)
        {
            var result = InvokeGeneric<T>("Deserialize", new []{ value });
            return (T)result;
        }

        public T Deserialize<T>(Stream stream, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;

            using var streamReader = new StreamReader(stream, encoding);
            using var jsonTextReader = new JsonTextReader(streamReader);

            return InvokeDeserialize<T>(jsonTextReader);
        }

        public void Serialize<T>(Stream stream, T instance, int bufferSize = 4096, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;

            // Client code needs to dispose this.
            const bool leaveOpen = true;

            using var writer = new StreamWriter(stream, encoding, bufferSize, leaveOpen: leaveOpen);
            using var jsonTextWriter = new JsonTextWriter(writer);

            InvokeSerialize(jsonTextWriter, instance);

            jsonTextWriter.Flush();
            stream.Position = 0;
        }
    }

    public sealed class CamelCasePropertyNamesContractResolver : ReflectedType
    {
        public CamelCasePropertyNamesContractResolver() :
            base("Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver, Newtonsoft.Json")
        {
        }

        public object NamingStrategy
        {
            get => GetProperty("NamingStrategy");
            set => SetProperty("NamingStrategy", value);
        }
    }
    
    public sealed class CamelCaseNamingStrategy : ReflectedType
    {
        public CamelCaseNamingStrategy() :
            base("Newtonsoft.Json.Serialization.CamelCaseNamingStrategy, Newtonsoft.Json")
        {
        }
    }

    public sealed class JsonTextWriter : ReflectedType, IDisposable
    {
        public JsonTextWriter(StreamWriter writer) :
            base("Newtonsoft.Json.JsonTextWriter, Newtonsoft.Json", writer)
        {
        }

        public void Flush()
        {
            Invoke("Flush");
        }
    }

    public sealed class JsonTextReader : ReflectedType, IDisposable
    {
        public JsonTextReader(StreamReader reader) :
            base("Newtonsoft.Json.JsonTextReader, Newtonsoft.Json", reader)
        {
        }
    }
}