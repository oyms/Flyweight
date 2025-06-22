Flyweight model code generator
===
This project generates code for the [Flyweight design pattern](https://en.wikipedia.org/wiki/Flyweight_pattern), 
which is used to minimize memory usage by sharing common data among multiple objects.

It is used with the [Skaar.Flyweight](https://www.nuget.org/packages/Skaar.Flyweight) library to create efficient, 
memory-conscious applications.

## Installation

In the `.csproj` file, add the following:

```xml
<ItemGroup>
  <PackageReference 
      Include="Skaar.Flyweight.CodeGeneration" Version="*"
      OutputItemType="Analyzer"
      ReferenceOutputAssembly="false"
  />
  <PackageReference Include="Skaar.Flyweight" Version="*" />
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

[Documentation on GitHub](https://github.com/oyms/Flyweight/blob/main/README.md)

![Icon](https://raw.githubusercontent.com/oyms/Flyweight/refs/heads/main/.idea/.idea.Flyweight/.idea/icon.svg)