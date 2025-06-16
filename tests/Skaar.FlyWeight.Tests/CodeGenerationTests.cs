using Shouldly;
using Skaar.Flyweight;

[assembly:GenerateFlyweightClass(nameof(TestNs.TestType), "TestNs")]
//[assembly:GenerateFlyweightClass("Illegal name", "TestNs")]

namespace Skaar.FlyWeight.Tests;

public class CodeGenerationTests
{
    [Fact]
    public void Equals_WithSameKey_ReturnsTrue()
    {
        var first = TestNs.TestType.Get("1");
        var second = TestNs.TestType.Get("1");
        first.ShouldBe(second);
        (first == second).ShouldBeTrue();
        ReferenceEquals(first, second).ShouldBeTrue();
    }
}