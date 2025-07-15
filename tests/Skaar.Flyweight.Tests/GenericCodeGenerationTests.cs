using Shouldly;
using Skaar.Flyweight;
using Xunit.Sdk;

[assembly: GenerateFlyweightClass<Skaar.Flyweight.Tests.TestValue>("TestNs.GenericTestType")]
[assembly: GenerateFlyweightClass<Skaar.Flyweight.Tests.TestValueIComparable>("TestNs.GenericTestType1")]

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
    
    [Fact]
    public void GeneratedGenericClass_ShouldImplementIComparable()
    {
        typeof(TestNs.GenericTestType1).Implements(typeof(IComparable<TestValueIComparable>)).ShouldBeTrue();
        typeof(TestTypeShouldImplementIComparable).Implements(typeof(IComparable<TestValueIComparable>)).ShouldBeTrue();
        typeof(TestTypeShouldImplementIComparable).Implements(typeof(IComparable<int>)).ShouldBeTrue();
        typeof(TestTypeShouldImplementIComparable).Implements(typeof(IComparable<int?>)).ShouldBeTrue();
    }
}

public record TestValue(int Value);

public record TestValueIComparable(int Value) : 
    IComparable<TestValueIComparable>, 
    IComparable<int>,
    IComparable<int?>
{
    public int CompareTo(TestValueIComparable? other)
    {
        if (other is null) return 1;
        return Value.CompareTo(other.Value);
    }

    public int CompareTo(int other)
    {
        return Value.CompareTo(other);
    }
    public int CompareTo(int? other)
    {
        if (other is null) return 1;
        return Value.CompareTo(other);
    }
}

[Flyweight<TestValue>]
partial class TestType3;

[Flyweight<TestValueIComparable>]
partial class TestTypeShouldImplementIComparable;
