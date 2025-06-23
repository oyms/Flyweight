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
    
    [Fact]
    public void ExtendedClass_Equals_WithSameKey_ReturnsTrue()
    {
        var first = TestType3.Get(new(1));
        var second = TestType3.Get(new(1));
        first.ShouldBe(second);
        (first == second).ShouldBeTrue();
        ReferenceEquals(first, second).ShouldBeTrue();
    }
}

public record TestValue(int Value);

[Flyweight<TestValue>]
partial class TestType3;
