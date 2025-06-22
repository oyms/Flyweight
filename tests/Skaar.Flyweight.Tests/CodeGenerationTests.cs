using  global;
using Shouldly;
using Skaar.Flyweight;

[assembly: GenerateFlyweightClass("TestNs.TestType")]
[assembly: GenerateFlyweightClass("TestNs.TestType")]
[assembly: GenerateFlyweightClass("TestNs.Subnamespace.TestType")]
[assembly: GenerateFlyweightClass(nameof(SomeClassName))]

namespace Skaar.Flyweight.Tests;

public class CodeGenerationTests
{
    [Fact]
    public void GeneratedClass_Equals_WithSameKey_ReturnsTrue()
    {
        var first = TestNs.TestType.Get("0");
        var second = TestNs.TestType.Parse("0");
        first.ShouldBe(second);
        (first == second).ShouldBeTrue();
        ReferenceEquals(first, second).ShouldBeTrue();
    }

    [Fact]
    public void ExtendedClass_Equals_WithSameKey_ReturnsTrue()
    {
        var first = TestType1.Get("1");
        var second = TestType1.Parse("1");
        first.ShouldBe(second);
        (first == second).ShouldBeTrue();
        ReferenceEquals(first, second).ShouldBeTrue();
    }
}
    

[Flyweight]
partial class TestType1;

[Flyweight]
partial class TestType2;
