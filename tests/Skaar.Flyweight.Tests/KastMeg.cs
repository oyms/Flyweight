using Skaar.Flyweight;

[assembly: GenerateFlyweightClass<Skaar.Flyweight.Tests.TestValue>("TestNs.GenericTestType")]

namespace Skaar.Flyweight.Tests;

public class GenericCodeGenerationTests
{
    
}

public record TestValue(int Value);