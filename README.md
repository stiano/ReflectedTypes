# ReflectedTypes

A simple (naive) library for doing easy reflection over types (that are not necessarily referenced by your project/library).

## Setup

Inherit from ReflectedType / ReflectedEnum and proxy method calls to the underlying object/instance.

```csharp
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
```

## Usage

```csharp
var formatting = new FormattingEnum(FormattingEnum.Options.Indented);
var jsonConvert = new JsonConvert();

var json = jsonConvert.SerializeObject(new Person() {Name = "Thorstein"}, formatting);
```
