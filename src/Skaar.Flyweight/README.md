Flyweight model
===
This project generates code for the [Flyweight design pattern](https://en.wikipedia.org/wiki/Flyweight_pattern),
which is used to minimize memory usage by sharing common data among multiple objects.

The type wraps a string value. The string values are reused.

It may be used with the Skaar.Flyweight.CodeGeneration library to simplify usage.

## Usage

Create classes that inherits from the `FlyweightBase` class.

```csharp
using Skaar.Flyweight;

[JsonConverter(typeof(FlyweightJsonConverter<MyFlyweight>))]
class MyFlyweight : FlyweightBase<MyFlyweight>, IFlyweightFactory<MyFlyweight>
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