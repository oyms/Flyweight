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

    [Fact]
    public void Get_WithFactoryAndPredicate_ReturnsSameInstance()
    {
        var predicate = new Predicate<TestValue>(v => v.Value == 2);
        var factory = () => new TestValue(2);
        
        var first = TestType3.Get(predicate, factory);
        var second = TestType3.Get(predicate, factory);
        
        first.ShouldBe(second);
    }
}

public record TestValue(int Value);

[Flyweight<TestValue>]
partial class TestType3;
