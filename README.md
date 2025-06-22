Flyweight model
===

<img alt="icon" style="width: 200px;" src="./resources/logo.svg" />

This is a library for reusing models to save memory.
This is useful when you have a lot of similar models.

The models can act like enum values, in the sense that
they can be enumerated over every possible value.

## Flyweight library

[![NuGet Version](https://img.shields.io/nuget/v/Skaar.Flyweight.svg)](https://www.nuget.org/packages/Skaar.Flyweight) 

This library contains a base class for the flyweight model, wrapping a string value.
The models are reused, so the same string value will not be stored multiple times.

It can be used with the [`Skaar.Flyweight.CodeGeneration`](#code-generator) library to simplify usage.

### Installation

You can install the library via NuGet:

```bash
dotnet add package Skaar.Flyweight
```

### Usage

Create classes that inherit from the `FlyweightBase` class.

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

To get an instance (outside JSON serialization), you can use the `Get` method:

```csharp
var myFlyweight = MyFlyweight.Get("myKey");
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

Add the code generation package to your .csproj file:

```xml
<ItemGroup>
  <PackageReference 
      Include="Skaar.Flyweight.CodeGeneration"
      OutputItemType="Analyzer"
      ReferenceOutputAssembly="false"
  />
  <PackageReference Include="Skaar.Flyweight" />
</ItemGroup>
```

## Usage

Add the `Flyweight` attribute to a partial class that you want to use as a flyweight.

```csharp

using Skaar.Flyweight;
[Flyweight] 
public partial class MyFlyweight;
```

Or use the `GenerateFlyweightClassAttribute` to generate a new flyweight class;

```csharp
[assembly: GenerateFlyweightClass("MyNamespace.MyFlyweight")]
```
