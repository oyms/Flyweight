using Shouldly;
using Skaar.Flyweight;

[assembly: GenerateFlyweightClass<Skaar.Flyweight.Tests.TestValue>("TestNs.GenericTestType")]

namespace Skaar.Flyweight.Tests;

public class GenericCodeGenerationTests
{
    [Fact]
    public void GeneratedGenericClass_Equals_WithSameKey_ReturnsTrue()
    {
        var first = TestNs.GenericTestType.Get(new TestValue(0));
        var second = TestNs.GenericTestType.Get(new TestValue(0));
        first.ShouldBe(second);
        (first == second).ShouldBeTrue();
        ReferenceEquals(first, second).ShouldBeTrue();
        first.GetInnerValue().Value.ShouldBe(0);
    }
}

public record TestValue(int Value);