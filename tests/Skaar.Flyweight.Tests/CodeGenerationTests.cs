using global;
using Shouldly;
using Skaar.Flyweight;

[assembly: GenerateFlyweightClass("TestNs.TestType")]
[assembly: GenerateFlyweightClass(nameof(Illegalname))]

namespace Skaar.Flyweight.Tests;

public class CodeGenerationTests
{
    [Fact]
    public void GeneratedClass_Equals_WithSameKey_ReturnsTrue()
    {
        var first = TestNs.TestType.Get("1");
        var second = TestNs.TestType.Get("1");
        first.ShouldBe(second);
        (first == second).ShouldBeTrue();
        ReferenceEquals(first, second).ShouldBeTrue();
    }

    [Fact]
    public void ExtendedClass_Equals_WithSameKey_ReturnsTrue()
    {
        var first = TestType1.Get("1");
        var second = TestType1.Get("1");
        first.ShouldBe(second);
        (first == second).ShouldBeTrue();
        ReferenceEquals(first, second).ShouldBeTrue();
    }
}
    
[Flyweight]
partial class TestType1;

