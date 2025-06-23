Flyweight model
===
This project generates code for the [Flyweight design pattern](https://en.wikipedia.org/wiki/Flyweight_pattern),
which is used to minimize memory usage by sharing common data among multiple objects.

The type wraps an inner value. The values are reused.

It may be used with the [code generation](https://www.nuget.org/packages/Skaar.Flyweight.CodeGeneration) library 
to simplify usage.

## Usage

Create classes that inherit from the `FlyweightBase` class.

For strings:

```csharp
using Skaar.Flyweight;

[JsonConverter(typeof(FlyweightJsonConverter<MyFlyweight>))]
class MyFlyweight : FlyweightBase<MyFlyweight>, IFlyweightFactory<MyFlyweight, string>
{
    private MyFlyweight(string key) : base(key)
    {
    }

    public static MyFlyweight Get(string key)
    {
        return Get(key, value => new MyFlyweight(value));
    }
}
```

For other types:

```csharp
using Skaar.Flyweight;

[JsonConverter(typeof(FlyweightJsonConverter<MyFlyweight>))]
class MyFlyweight : FlyweightBase<MyFlyweight, ValueType>, IFlyweightFactory<MyFlyweight, ValueType>
{
    private MyFlyweight(ValueType key) : base(key)
    {
    }

    public static MyFlyweight Get(ValueType key)
    {
        return Get(key, value => new MyFlyweight(value));
    }
}

record ValueType(bool BoolValue, int IntValue);
```

[Documentation on GitHub](https://github.com/oyms/Flyweight/blob/main/README.md)

![Icon](https://raw.githubusercontent.com/oyms/Flyweight/refs/heads/main/.idea/.idea.Flyweight/.idea/icon.svg)