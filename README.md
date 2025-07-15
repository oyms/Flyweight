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

    public static MyFlyweight Get(string key) => GetOrCreate(key, value => new MyFlyweight(value));
    public static MyFlyweight Get(Predicate<string> predicate, Func<string> factory) => GetOrCreate(predicate, () => new MyFlyweight(factory()));
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

    public static MyFlyweight Get(ValueType key) => GetOrCreate(key, value => new MyFlyweight(value));
    public static MyFlyweight Get(Predicate<ValueType> predicate, Func<ValueType> factory) => GetOrCreate(predicate, () => new TestType(factory()));
}

record ValueType(bool BoolValue, int IntValue);
```

To get an instance (outside JSON serialization), you can use the `Get` method:

```csharp
var myFlyweight = MyFlyweight.Get(myValue);
```

To get an instance without creating the inner value unneccessary, use `Get` with a predicate

```csharp

var myFlyweight = MyFlyweight.Get(x => x.IntValue == 42 && x.BoolValue, () => new ValueType(true, 42));
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

### Usage

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

### Generated implementations for the generic variant

The generated class will have explicit implementations of all `IComparable<T>` that the inner DataType implements.

## Using scope

By using a `FlyweightScope`, the internal catalogue of values created within the scope is purged.
This allows for memory to be reclaimed when the scope is disposed and the instances are no longer used.

```csharp
// first and second will by the same instance (ReferenceEquals will be true).
// third will be a new instance, as it is outside the scope.
using(FlyWeightScope.Create())
{
    var first = MyFlyweight.Get("a");
    var second = MyFlyweight.Get("a");
}

var third = MyFlyweight.Get("a");
```