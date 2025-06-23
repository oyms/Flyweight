Flyweight model
===

<img alt="icon" style="width: 200px;" src="./resources/logo.svg" />

```csharp
[Flyweight]
partial class MyFlyweight;

var a = MyFlyweight.Get("some string");
var b = MyFlyweight.Get("some string");

Console.WriteLine(ReferenceEquals(a, b)); // True, both refer to the same instance
```

This is a library for reusing models to save memory.
This is useful when you have a lot of similar models. For instance, when deserializing large JSON data
where the same string values are repeated many times, or when you have a lot of enum-like values.

The models can act like enum values, in the sense that
they can be enumerated over every possible value.

[![Static Badge](https://img.shields.io/badge/Wikipedia-Flyweight_Design_Pattern-blue?label=Wikipedia&link=https%3A%2F%2Fen.wikipedia.org%2Fwiki%2FFlyweight_pattern)](https://en.wikipedia.org/wiki/Flyweight_pattern)

## Flyweight library

[![NuGet Version](https://img.shields.io/nuget/v/Skaar.Flyweight.svg)](https://www.nuget.org/packages/Skaar.Flyweight) 

This library contains a base class for the flyweight model, wrapping an inner value.
The models are reused, so the same inner value will not be stored multiple times.

It can be used with the [`Skaar.Flyweight.CodeGeneration`](#code-generator) library to simplify usage.

### Installation

You can install the library via NuGet:

```bash
dotnet add package Skaar.Flyweight
```

### Usage

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

To get an instance (outside JSON serialization), you can use the `Get` method:

```csharp
var myFlyweight = MyFlyweight.Get(myValue);
```

To get all instances of the flyweight, you can use the `GetAll` method:

```csharp
foreach (var instance in MyFlyweight.AllValues)
{
    Console.WriteLine(instance);
}
```

## Code generator

[![NuGet Version](https://img.shields.io/nuget/v/Skaar.Flyweight.CodeGeneration.svg)](https://www.nuget.org/packages/Skaar.Flyweight.CodeGeneration) 

This library generates code for the flyweight model.

### Installation

Add both the code generation package and the Flyweight library to your .csproj file:

```xml
<ItemGroup>
    <PackageReference Include="Skaar.Flyweight" Version="*" />
    <PackageReference Include="Skaar.Flyweight.CodeGeneration" Version="*" />
</ItemGroup>
```

## Usage

Add the `Flyweight` attribute to a partial class that you want to use as a flyweight.

For strings:

```csharp
using Skaar.Flyweight;
[Flyweight] 
public partial class MyFlyweight;
```

For other types:

```csharp
using Skaar.Flyweight;
[Flyweight<DataType>] 
public partial class MyFlyweight;
public record DataType;
```


Or use the `GenerateFlyweightClassAttribute` to generate a new flyweight class;

```csharp
[assembly: GenerateFlyweightClass("MyNamespace.MyFlyweight")] // for strings
[assembly: GenerateFlyweightClass<DataType>("MyNamespace.MyOtherFlyweight")]
```
